using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Avatar = GameEntities.Entities.Avatar;

public class RankingView : MonoBehaviour
{
	public AvatarView AvatarViewPrefab;

	private List<AvatarView> _avatarList = new();

	public void RefreshRanking(Avatar avatar)
	{
		for (int i = 0; i < _avatarList.Count; i++)
		{
			if (string.Equals(_avatarList[i].Name.text, avatar.Name))
			{
				_avatarList[i].Score.text = avatar.Score.ToString();
				break;
			}
		}

		_avatarList = _avatarList.OrderByDescending(x => uint.Parse(x.Score.text)).ToList();

		for (int i = 0; i < _avatarList.Count; i++)
		{
			_avatarList[i].transform.SetSiblingIndex(i);
		}
	}

	public void AddAvatar(Avatar newAvatar)
	{
		var newAvatarView = Instantiate(AvatarViewPrefab);
		newAvatarView.Name.text = newAvatar.Name;
		newAvatarView.Score.text = newAvatar.Score.ToString();

		newAvatarView.transform.SetParent(transform, false);

		_avatarList.Add(newAvatarView);
	}

	public void RemoveAvatar(Avatar newAvatar)
	{
		for (int i = 0; i < _avatarList.Count; i++)
		{
			if (string.Equals(_avatarList[i].Name.text, newAvatar.Name))
			{
				Destroy(_avatarList[i].gameObject);
				_avatarList.Remove(_avatarList[i]);
				break;
			}
		}
	}
}
