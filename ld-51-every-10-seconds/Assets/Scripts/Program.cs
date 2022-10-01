using CustomSerializer;
using ENetTransporter;
using GameBase;
using GameEntities;
using GameEntities.Entities;
using GameEntities.Messages;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Program : MonoBehaviour
{
	public GameView GameView;

	ENetTransporterClient _transporter;

	GameState _gameState;

	EventManager _eventManager;

	Dictionary<BaseEntity, MinesweeperGame> _games = new Dictionary<BaseEntity, MinesweeperGame>();

	ConcurrentStack<long> _ownerWaitForEntity = new ConcurrentStack<long>();

	//ConcurrentStack<BaseEntity> createStack = new ConcurrentStack<BaseEntity>();

	//ConcurrentStack<BaseEntity> destroyStack = new ConcurrentStack<BaseEntity>();

	private bool mainThreadRefresh = false;

	private void Start()
	{
		ENet.Library.Initialize();

		var serializer = new CustomJsonSerialize();

		_transporter = new ENetTransporterClient(serializer);

		TypeManager.TypeManager.RegisterClass<GameUpdateMessage>();
		TypeManager.TypeManager.RegisterClass<GameStateMessage>();
		TypeManager.TypeManager.RegisterClass<OwnerMessage>();

		_gameState = new GameState();

		_gameState.OnAdd += (sender, e) =>
		{
			//createStack.Push(e);
		};

		_gameState.OnAssignOwner += (sender, e) =>
		{
			_ownerWaitForEntity.Push(e.EntityID);
		};

		_gameState.OnRemove += (sender, e) =>
		{
			//destroyStack.Push(e);
		};

		_eventManager = new EventManager();
		//_eventManager.Subscribe<ProxyBeginAttackMessage>("ProxyBeginAttackMessage ", OnProxyBeginAttackMessage);
		//_eventManager.Subscribe<ProxyAttackMessage>("ProxyAttackMessage", OnProxyAttackMessage);
		_eventManager.Subscribe<GameUpdateMessage>("GameUpdateMessage ", OnGameUpdateMessage);
		//_eventManager.Subscribe<GameUpdateMessage>("GameStateMessage ", OnGameStateMessage);

		_transporter.ConnectHandle += (sender, e) =>
		{
			// Enviar nickname
			//_transporter.Send(new RequestGameStateMessage());
		};

		// Escutando mensagens chegando
		_transporter.MessageReceived += (sender, e) =>
		{
			e.Message.Execute(e.Sender, _transporter, _gameState, _eventManager);
			//DumpGameState();

			Debug.Log(e.Message.ToString());

			switch (e.Message)
			{
				case GameUpdateMessage gameUpdateMessage:
					mainThreadRefresh = true;
					break;
			}
		};

		_transporter.Start(ConfigReader.GetENetTransporterConfig());
	}

	private void OnGameUpdateMessage(GameUpdateMessage message)
	{
		var game = _gameState.Get<MinesweeperGame>(message.GameId);
		GameView.RefreshMatrixView(game);
		Debug.Log("Refresh EVENT");
	}

	//private void OnGameStateMessage(GameStateMessage message)
	//{
	//	var game = _gameState.Get<BaseEntity>();
	//	// already updated
	//}

	//private void OnProxyBeginAttackMessage(ProxyBeginAttackMessage message)
	//{
	//	var entity = _gameState.Get<BaseEntity>(message.ID);
	//	if (_avatarGameObject.TryGetValue(entity, out var avatar))
	//	{
	//		avatar.IsAttack = true;
	//	}
	//}

	//private void OnProxyAttackMessage(ProxyAttackMessage message)
	//{
	//	var entityOwner = _gameState.Get<BaseEntity>(message.OwnerID);
	//	if (_avatarGameObject.TryGetValue(entityOwner, out var owner))
	//	{
	//		owner.IsDoHit = true;
	//	}

	//	var entityTarget = _gameState.Get<BaseEntity>(message.TargetID);
	//	if (_avatarGameObject.TryGetValue(entityTarget, out var target))
	//	{
	//		target.IsTakeHit = true;
	//		target.HitDamage = message.Damage;
	//	}
	//}

	//void DumpGameState()
	//{
	//	if (_games.Count > 0)
	//	{
	//		foreach (var kv in _gameState.Entities)
	//		{
	//			if (kv.GetType() == typeof(Ball))
	//			{
	//				_games[(Ball)kv].ChangeColor((ConsoleColor)((Ball)kv).Color);
	//			}
	//		}
	//	}
	//}

	private void Update()
	{
		if (mainThreadRefresh)
		{
			mainThreadRefresh = false;
			var game = _gameState.Get<MinesweeperGame>(_gameState.MyEntities.ElementAt(0));
			GameView.RefreshMatrixView(game);
			Debug.Log("Refresh part 2");
		}

		var hasUpdate = false;
		//while (createStack.Count > 0)
		{
			//if (createStack.TryPop(out var entity))
			//{

			//	if (entity.GetType() == typeof(Ball))
			//	{
			//		var newObject = Instantiate(ballPrefab);
			//		newObject.transform.position = _spawnPoint[spawnPosition++].transform.position;
			//		_games.Add(entity, newObject);
			//	}
			//	else if (entity.GetType() == typeof(GameEntities.Avatar))
			//	{
			//		var avatar = (GameEntities.Avatar)entity;
			//		var newObject = Instantiate(avatarPrefab);
			//		newObject.transform.position = new UnityEngine.Vector3(avatar.Position.x, avatar.Position.y, avatar.Position.z);
			//		_avatarGameObject.Add(entity, newObject);
			//		newObject.EntityID = avatar.Id;
			//		newObject._baseGameState = _gameState;
			//		newObject._transporter = _transporter;
			//		newObject.Health = avatar.Health;
			//		newObject.UpdateHealthBar();
			//	}
			//	hasUpdate = true;
			//}
		}


		//while (destroyStack.Count > 0)
		{
			//if (destroyStack.TryPop(out var entity))
			//{
			//	if (entity.GetType() == typeof(Ball))
			//	{
			//		_games.TryGetValue(entity, out var value);
			//		_games.Remove(entity);
			//		Destroy(value.gameObject);
			//	}
			//	else if (entity.GetType() == typeof(GameEntities.Avatar))
			//	{
			//		_avatarGameObject.TryGetValue(entity, out var value);
			//		_avatarGameObject.Remove(entity);
			//		Destroy(value.gameObject);
			//	}
			//	hasUpdate = true;
			//}
		}

		if (_ownerWaitForEntity.Count > 0)
		{
			if (_ownerWaitForEntity.TryPeek(out var id))
			{
				var game = _gameState.Get<MinesweeperGame>(id);

				GameView.Initialize(game);

				GameView.OnPlayClick += GameView_OnPlayClick;
				GameView.OnMarkClick += GameView_OnMarkClick;

				_ownerWaitForEntity.TryPop(out var owner);

				hasUpdate = true;
			}
		}

		//if (hasUpdate)
		//{
		//	DumpGameState();
		//}
	}

	private void GameView_OnMarkClick(int index)
	{
		_transporter.Send(new MarkMessage { GameId = _gameState.MyEntities.ElementAt(0), Index = index });
	}

	private void GameView_OnPlayClick(int index)
	{
		_transporter.Send(new PlayMessage { GameId = _gameState.MyEntities.ElementAt(0), Index = index });
	}

	private void OnDestroy()
	{
		_transporter.Stop();
		_transporter.Dispose();
		ENet.Library.Deinitialize();
	}
}
