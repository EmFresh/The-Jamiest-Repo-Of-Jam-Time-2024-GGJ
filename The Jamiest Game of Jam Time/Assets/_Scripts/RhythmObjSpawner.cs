using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.TextCore.Text;

public class RhythmObjSpawner : MonoBehaviour
{
	public SongTrack track;
	public Transform parentObj;
	public float reactTime = 2.1f;
	public float speed = 1.0f;
	 
	public List<AudioClip> clips = new List<AudioClip>();


	int last = 0;
	bool init = true;
	List<Tuple<float, NoteData>> timings = new List<Tuple<float, NoteData>>();

	private void SpawnUpdate()
	{
		if(!track) return;

		if(init)
		{
			track.beats.Sort((a, b) => { return a?.noteData.hitTimings[0].CompareTo(b?.noteData.hitTimings[0]) ?? 1; });
			foreach(var beat in track.beats)
				if(beat?.noteData.hitTimings[0] != 0 && (beat?.noteData.hitTimings[0] ?? float.PositiveInfinity) != float.PositiveInfinity)
					timings.Add(new Tuple<float, NoteData>(beat.noteData.hitTimings[0], beat.noteData));

			init = false;
		}

		GameObject createEnemy(int index)
		{
			//Find floor location
			var casts = Physics.RaycastAll(transform.position + new Vector3(-3, 0, 0), -transform.up);
			var point = transform.position;
			foreach(var cast in casts)
				point = cast.point;

			//create object
			GameObject obj = null;
			obj = Instantiate(track.enemyPrefabs[index], parentObj);
			var box = obj.GetComponent<Collider>().bounds;
			obj.transform.localPosition = point + box.center + new Vector3(0, obj.GetComponent<Collider>().bounds.extents.y, 0);
			obj.transform.localRotation = Quaternion.Euler(0, 0, 0);

			return obj;
		}


		GameObject createPlatform(int index)
		{
			//find floor
			var casts = Physics.RaycastAll(transform.position + new Vector3(-3, 0, 0), -transform.up);
			//foreach(var cast in casts)
			//	if(cast.transform.GetComponent<RhythmObjMovement>())
			//		return createEnemy(index);

			//Get location
			var point = transform.position;
			foreach(var cast in casts.Reverse())
				point = cast.point;


			//Place in one of 3 heights

			var height = transform.position.y * (1.0f / 3);
			//print($"height: {height}");


			System.Random ran = new System.Random();

			GameObject obj = null;
			obj = Instantiate(track.enemyPrefabs[index], parentObj);
			var box = obj.GetComponent<Collider>().bounds;

			obj.transform.localPosition = point - box.center + new Vector3(0, height * ran.Next(1, 3), 0);
			obj.transform.localRotation = Quaternion.Euler(0, 0, 0);



			return obj;
		}

		//print(last);
		for(int count = last; count < timings.Count; ++count)
		{
			var time = timings[count];

			//print(clip.time - time.Item1);
			if(clip.time/*replacing song time*/ - time.Item1 >= reactTime) // top bound for enemy
				continue;


			if(clip.time/*replacing song time*/ - time.Item1 >= -.1) // within bound
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
				case (int)HitType.PLATFORM1:
					obj = createPlatform(4);
					break;
				case (int)HitType.PLATFORM2:
					obj = createPlatform(5);
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

				System.Random rand = new System.Random();
				clip.PlayOneShot(clips[rand.Next(clips.Count)]);
			}
		}
	}

	public AudioSource clip = null;

	void Update()
	{
		SpawnUpdate();
	}
}
