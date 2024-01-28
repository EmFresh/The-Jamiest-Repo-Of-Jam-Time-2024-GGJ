using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.TextCore.Text;

public class RhythmObjSpawner : MonoBehaviour
{
	public SongTrack track;
	public Transform parentObj;
	public float reactTime = 2.1f;
	public float speed = 1.0f;

	int last = 0, texNum = 0;
	bool init = true;
	List<Tuple<float, NoteData>> timings = new List<Tuple<float, NoteData>>();

	private void SpawnUpdate()
	{
		if(!track) return;

		if(init)
		{
			track.beats.Sort((a, b) => { return a?.noteData.hitTimings[0].CompareTo(b?.noteData.hitTimings[0]) ?? 1; });
			foreach(var beat in track.beats)
				if((beat?.noteData.hitTimings[0] ?? float.PositiveInfinity) != float.PositiveInfinity)
					timings.Add(new Tuple<float, NoteData>(beat.noteData.hitTimings[0], beat.noteData));

			init = false;
		}

		GameObject createEnemy(int index)
		{
			var casts = Physics.RaycastAll(transform.position + new Vector3(-5, 0, 0), -transform.up);
			var point = transform.position;
			foreach(var cast in casts.Reverse())
				point = cast.point;

			GameObject obj = null;
			obj = Instantiate(track.enemyPrefabs[index], parentObj);
			obj.transform.localPosition = point + new Vector3(0, obj.GetComponent<Collider>().bounds.extents.y, 0);
			obj.transform.localRotation = Quaternion.Euler(0, 0, 0);

			return obj;
		}


		GameObject createPlatform(int index)
		{
			var casts = Physics.RaycastAll(transform.position + new Vector3(-5, 0, 0), -transform.up);
			foreach(var cast in casts)
				if(cast.transform.GetComponent<RhythmObjMovement>())
					return createEnemy(index);

			var point = transform.position;
			foreach(var cast in casts.Reverse())
				point = cast.point;

			GameObject obj = null;
			obj = Instantiate(track.enemyPrefabs[index], parentObj);
			obj.transform.localPosition = point;
			obj.transform.localRotation = Quaternion.Euler(0, 0, 0);

			return obj;
		}


		for(int count = last; count < timings.Count; ++count)
		{
			var time = timings[count];

			if(clip.time/*replacing song time*/ - time.Item1 >= reactTime) // top bound for enemy
				break;


			if(clip.time/*replacing song time*/ - time.Item1 >= -.05) // within bound
			{
				GameObject obj = null;

				switch((int)time.Item2.hitTypes[0])
				{
				case (int)HitType.TEST1:
					obj = createEnemy(0);
					break;
				case (int)HitType.TEST2:
					obj = createEnemy(1);
					break;
				case (int)HitType.TEST3:
					obj = createEnemy(2);
					break;
				case (int)HitType.TEST4:
					obj = createEnemy(3);
					break;
				}

				if(obj == null)
				{
					last = count + 1;
					continue;
				}

				var mov = obj.AddComponent<RhythmObjMovement>();
				mov.dir = -obj.transform.right;
				mov.speed = speed;


				last = count + 1;
			}
		}
	}

	public AudioSource clip = null;

	void Update()
	{
		SpawnUpdate();
	}
}
