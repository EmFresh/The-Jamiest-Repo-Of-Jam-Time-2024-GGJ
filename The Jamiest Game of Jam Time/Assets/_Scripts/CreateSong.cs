using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;

using UnityEngine;

using static UnityEngine.InputSystem.InputAction;

public class CreateSong : MonoBehaviour
{
	public SongTrack track;

	public new AudioSource audio;



	// Start is called before the first frame update
	void Awake()
	{
#if UNITY_EDITOR

		var note = new List<HitType> { HitType.TEST1, HitType.TEST2, HitType.TEST3, HitType.TEST4, HitType.PLATFORM1, HitType.PLATFORM2, };
		//foreach(var beat in track.beats.ToList())
		//	DestroyImmediate(beat);

		track.beats.Clear();

		//int num = 1;

		NoteHitting.onNotePressed.AddListener((ht) =>
		{

			var tmp = ScriptableObject.CreateInstance<BeatData>();

			System.Random ran = new System.Random();
			tmp.noteData = new NoteData();
			tmp.noteData.hitTimings = new List<float> { audio.time };

			tmp.noteData.hitTypes = new List<HitType> { note[ran.Next(0, note.Count)] };
			track.beats.Add(tmp);

			print("note hit");

			//string path = $"Assets/_scripts/SongNotes/Note{num++}.asset";//realized that number is now a static variable even out of scope
			//AssetDatabase.CreateAsset(tmp, path);//uncomment wen creating later 

		});

#endif
	}

	public void Save(CallbackContext ctx)
	{
		if(!ctx.performed) return;

#if UNITY_EDITOR
		if(!enabled) return;

		int num = 1;
		var notes = track.beats;
		foreach(var tmp in notes)
		{
		//	Application.dataPath;
			string path = $"Assets/_audio/song 2/Note{num++}.asset";
			while(File.Exists(path))
				path = $"Assets/_audio/song 2/Note{num++}.asset";

			AssetDatabase.CreateAsset(tmp, path);//uncomment wen creating later 
		}
#endif
	}

}
