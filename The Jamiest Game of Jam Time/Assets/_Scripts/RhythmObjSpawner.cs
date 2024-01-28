using System;
using System.Collections;
using System.Collections.Generic;

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
			GameObject obj = null;
			obj = Instantiate(track.enemyPrefabs[index], parentObj);
			obj.transform.localPosition = transform.position;
			obj.transform.localRotation *= Quaternion.Euler(0, 0, 0);
			return obj;
		}

		for(int count = last; count < timings.Count; ++count)
		{
			var time = timings[count];

			if(clip.time/*replacing song time*/ - time.Item1 >= reactTime) // top bound for enemy
				break;


			if(clip.time/*replacing song time*/ - time.Item1 >= -.05) // within bound
			{
				//	print("created Object!!");

				//var obj = Instantiate(track.enemyPrefabs[0], new Vector3(0, 0, time.Item1), Quaternion.Euler(0, 180, 0), parentObj);

				GameObject obj = null;
				//var notespace = parentObj.transform.GetChild(0).GetComponent<Collider>().bounds.extents.x / 2;

				switch((int)time.Item2.hitTypes[0] )
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
					//	case HitType.TEST4:
					//		obj = createEnemy(1);
					//		break;
				}

				if(obj == null)
				{
					last = count + 1; 
					continue;
				}
				//	obj.transform.localPosition -= (Vector3)(Vector2)obj.GetComponentInChildren<MeshFilter>().mesh.bounds.min;

			//	var act = obj.AddComponent<RhythmObjActions>();
				var mov = obj.AddComponent<RhythmObjMovement>();


				mov.dir = -obj.transform.right;
				mov.speed = speed;

				//act.noteData = time.Item2;
				//act.reactTime = reactTime;
				//act.hitModel = track.notePrefabs[0];
				//act.clip = clip;

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
