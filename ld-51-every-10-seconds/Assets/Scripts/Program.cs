using CustomSerializer;
using ENetTransporter;
using GameBase;
using GameEntities;
using GameEntities.Entities;
using GameEntities.Messages;
using System.Collections.Concurrent;
using UnityEngine;
using Avatar = GameEntities.Entities.Avatar;

public class Program : MonoBehaviour
{
	public GameView GameView;
	public RankingView RankingView;

	ENetTransporterClient _transporter;

	GameState _gameState;

	EventManager _eventManager;

	//ConcurrentStack<long> _ownerWaitForEntity = new ConcurrentStack<long>();

	private readonly ConcurrentStack<BaseEntity> _createStack = new ConcurrentStack<BaseEntity>();

	private readonly ConcurrentStack<BaseEntity> _destroyStack = new ConcurrentStack<BaseEntity>();

	private readonly ConcurrentStack<Avatar> _scoreUpdateStack = new ConcurrentStack<Avatar>();

	private bool _mainThreadRefreshGame = false;
	private bool _mainThreadStartCount = false;

	private void Start()
	{
		GameView.Initialize(new MinesweeperGame(10, 10));
		GameView.OnPlayClick += GameView_OnPlayClick;
		GameView.OnMarkClick += GameView_OnMarkClick;

		ENet.Library.Initialize();

		var serializer = new CustomJsonSerialize();

		_transporter = new ENetTransporterClient(serializer);

		TypeManager.TypeManager.RegisterClass<GameUpdateMessage>();
		TypeManager.TypeManager.RegisterClass<AvatarUpdateMessage>();
		TypeManager.TypeManager.RegisterClass<GameChangeMessage>();
		TypeManager.TypeManager.RegisterClass<AvatarStateMessage>();
		TypeManager.TypeManager.RegisterClass<OwnerMessage>();
		TypeManager.TypeManager.RegisterClass<CreateAvatarMessage>();
		TypeManager.TypeManager.RegisterClass<DestroyAvatarMessage>();

		_gameState = new GameState();

		_gameState.OnAdd += (sender, e) =>
		{
			_createStack.Push(e);
		};

		//_gameState.OnAssignOwner += (sender, e) =>
		//{
		//	_ownerWaitForEntity.Push(e.EntityID);
		//};

		_gameState.OnRemove += (sender, e) =>
		{
			_destroyStack.Push(e);
		};

		_eventManager = new EventManager();
		//_eventManager.Subscribe<ProxyBeginAttackMessage>("ProxyBeginAttackMessage ", OnProxyBeginAttackMessage);
		//_eventManager.Subscribe<ProxyAttackMessage>("ProxyAttackMessage", OnProxyAttackMessage);
		_eventManager.Subscribe<GameUpdateMessage>("GameUpdateMessage ", OnGameUpdateMessage);
		//_eventManager.Subscribe<GameUpdateMessage>("GameStateMessage ", OnGameStateMessage);

		_transporter.ConnectHandle += (sender, e) =>
		{
			// Enviar nickname
			_transporter.Send(new RequestCreateAvatarMessage
			{
				Name = StartGame.Nickname
			});
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
					_mainThreadRefreshGame = true;
					break;

				case GameChangeMessage gameUpdateMessage:
					_mainThreadStartCount = true;
					_mainThreadRefreshGame = true;
					break;

				case AvatarUpdateMessage avatarUpdateMessage:
					_scoreUpdateStack.Push(_gameState.Get<Avatar>(avatarUpdateMessage.AvatarID));
					break;

					//case AvatarStateMessage avatarStateMessage:
					//	mainThreadRefresh = true;
					//	break;

					//case OwnerMessage ownerMessage:
					//	mainThreadRefresh = true;
					//	break;

					//case CreateAvatarMessage createAvatarMessage:
					//	mainThreadRefresh = true;
					//	break;

					//case DestroyAvatarMessage destroyAvatarMessage:
					//	mainThreadRefresh = true;
					//	break;
			}
		};

		_transporter.Start(ConfigReader.GetENetTransporterConfig());
	}

	private void OnGameUpdateMessage(GameUpdateMessage message)
	{
		var game = _gameState.Get<MinesweeperGame>(message.GameID);
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
		if (_mainThreadRefreshGame)
		{
			_mainThreadRefreshGame = false;
			GameView.RefreshMatrixView(_gameState.MyGame);
		}

		if (_mainThreadStartCount)
		{
			_mainThreadStartCount = false;
			GameView.StartCount();
		}

		while (_scoreUpdateStack.Count > 0)
		{
			if (_scoreUpdateStack.TryPop(out var avatar))
			{
				RankingView.RefreshRanking(avatar);
			}
		}

		var hasUpdate = false;
		while (_createStack.Count > 0)
		{
			if (_createStack.TryPop(out var entity))
			{
				if (entity is Avatar avatar)
				{
					Debug.Log($"Create: {avatar.Name}: {avatar.ID}");

					RankingView.AddAvatar(avatar);

					//var avatar = (GameEntities.Avatar)entity;
					//var newObject = Instantiate(avatarPrefab);
					//newObject.transform.position = new UnityEngine.Vector3(avatar.Position.x, avatar.Position.y, avatar.Position.z);
					//_avatarGameObject.Add(entity, newObject);
					//newObject.EntityID = avatar.Id;
					//newObject._baseGameState = _gameState;
					//newObject._transporter = _transporter;
					//newObject.Health = avatar.Health;
					//newObject.UpdateHealthBar();
				}
				hasUpdate = true;
			}
		}


		while (_destroyStack.Count > 0)
		{
			if (_destroyStack.TryPop(out var entity))
			{
				if (entity is Avatar avatar)
				{
					Debug.Log($"Destroy: {avatar.Name}: {avatar.ID}");

					RankingView.RemoveAvatar(avatar);

					//_avatarGameObject.TryGetValue(entity, out var value);
					//_avatarGameObject.Remove(entity);
					//Destroy(value.gameObject);
				}
				hasUpdate = true;
			}
		}

		//if (_ownerWaitForEntity.Count > 0)
		//{
		//	if (_ownerWaitForEntity.TryPeek(out var id))
		//	{
		//		// Teoricamente isso pode ser feito logo no start
		//		GameView.Initialize(new MinesweeperGame(10, 10));

		//		GameView.OnPlayClick += GameView_OnPlayClick;
		//		GameView.OnMarkClick += GameView_OnMarkClick;

		//		_ownerWaitForEntity.TryPop(out var owner);

		//		hasUpdate = true;
		//	}
		//}

		//if (hasUpdate)
		//{
		//	DumpGameState();
		//}
	}

	private void GameView_OnMarkClick(int index)
	{
		_transporter.Send(new MarkMessage { GameID = _gameState.MyGame.ID, Index = index });
	}

	private void GameView_OnPlayClick(int index)
	{
		_transporter.Send(new PlayMessage { GameID = _gameState.MyGame.ID, Index = index });
	}

	private void OnDestroy()
	{
		_transporter.Stop();
		_transporter.Dispose();
		ENet.Library.Deinitialize();
	}
}
