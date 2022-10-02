using GameBase;
using GameEntities.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace GameEntities
{

	public class GameState : BaseGameState
	{
		private readonly ConcurrentDictionary<long, BaseEntity> _entities = new ConcurrentDictionary<long, BaseEntity>();

		public override ICollection<BaseEntity> Entities => _entities.Values;

		public ConcurrentDictionary<IBasePeer, long> EntityOwnerMap { get; } = new ConcurrentDictionary<IBasePeer, long>();

		public ConcurrentDictionary<IBasePeer, long> OwnerToGameMap { get; } = new ConcurrentDictionary<IBasePeer, long>();

		// TODO: client gameMemory
		private readonly List<long> _myEntities = new List<long>();

		public override ICollection<long> MyEntities => (ICollection<long>)_myEntities;

		public MinesweeperGame MyGame = new MinesweeperGame(10, 10);

		public event EventHandler<BaseEntity> OnAdd;

		public event EventHandler<BaseEntity> OnRemove;

		public event EventHandler<AssignOwnerEventArgs> OnAssignOwner;

		public override void Add(BaseEntity entity)
		{
			// TODO: rever solução.
			// Problema original - Ao connectar um gameClient no server, o server:
			//  - criar um avatar é instanciado => envia mensagem de create avatar
			//  - envia o gameState para o client
			if (!_entities.TryGetValue(entity.ID, out var _))
			{
				_entities[entity.ID] = entity;
				OnAdd?.Invoke(this, _entities[entity.ID]);
			}
		}

		public override void Remove(BaseEntity entity)
		{
			if (_entities.TryRemove(entity.ID, out var result))
			{
				OnRemove?.Invoke(this, result);
			}
		}

		public override T Get<T>(long entityID)
		{
			if (_entities.TryGetValue(entityID, out var entity))
			{
				return entity as T;
			}

			return null;
		}

		// TODO: improve error handling
		public override bool TryAssignOwner(long entityID, IBasePeer owner = null)
		{
			if (owner == null)
			{
				if (_myEntities.Contains(entityID))
				{
					return false;
				}

				_myEntities.Add(entityID);
				OnAssignOwner?.Invoke(this, new AssignOwnerEventArgs(owner, entityID));
				return true;
			}
			else
			{
				var result = EntityOwnerMap.TryAdd(owner, entityID);
				OnAssignOwner?.Invoke(this, new AssignOwnerEventArgs(owner, entityID));
				return result;
			}
		}

		public override string ToString()
		{
			var builder = new StringBuilder();
			builder.AppendLine("GAME MEMORY");

			builder.AppendLine("\nEntities Data:");
			foreach (var kv in _entities)
			{
				builder.AppendLine($"{kv.Value.GetType()} 0x{kv.Value.ID:x16} ");

				if (_myEntities.Count > 0)
				{
					builder.AppendLine($"Owner={_myEntities.Contains(kv.Value.ID)} ");
				}

				if (kv.Value is Avatar avatar)
				{
					builder.AppendLine(avatar.Name);
				}
				else if (kv.Value is MinesweeperGame minesweeperGame)
				{
					builder.AppendLine(minesweeperGame.ToString());
					builder.AppendLine("============");
					builder.AppendLine(minesweeperGame.ToStringPlayer());
				}

				builder.AppendLine();
			}

			if (!EntityOwnerMap.IsEmpty)
			{
				builder.AppendLine("\nOwnerMap");
				foreach (var kv in EntityOwnerMap)
				{
					builder.AppendLine($"BasePeer={kv.Key} Entity={kv.Value}");
				}
			}
			builder.Append("\n----------------------------------------------------------------\n");

			return builder.ToString();
		}
	}
}
