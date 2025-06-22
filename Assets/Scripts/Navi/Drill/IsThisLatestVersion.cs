using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using TMPro;
using Lean.Gui;

public class IsThisLatestVersion : MonoBehaviour
{
    public LeanButton button;

    void Start()
    {
        GetLatestVersion();
    }

    void SetText(string latestVersion, string url, string date)
    {
        if (latestVersion != Application.version)
        {
            TextMeshProUGUI text = gameObject.GetComponent<TextMeshProUGUI>();

            text.text = $"● 最新のバージョンがあります ●\n● version {latestVersion} ●\n● date {date} ●";
            text.enabled = true;

            button.OnClick.AddListener(() => Application.OpenURL(url));
            button.gameObject.SetActive(true);
        }
    }

    void GetLatestVersion() {
        // Firebaseの依存関係をチェックして初期化

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
                Debug.Log("Firebase Firestore initialized successfully.");

                // 特定のドキュメントからデータを取得
                DocumentReference docRef = db.Collection("Version").Document("Version");
                docRef.GetSnapshotAsync().ContinueWithOnMainThread(snapshotTask =>
                {
                    if (snapshotTask.IsCompleted)
                    {
                        DocumentSnapshot snapshot = snapshotTask.Result;
                        if (snapshot.Exists)
                        {
                            Debug.Log($"Document data for {snapshot.Id} document:");

                            // 特定のフィールドの値を取得
                            if (snapshot.TryGetValue("Latest", out string version) && snapshot.TryGetValue("URL", out string url) && snapshot.TryGetValue("Date", out string date))
                            {
                                //Debug.Log($"Latest: {version}");
                                SetText(version, url, date);
                            }
                            else
                            {
                                Debug.Log("Field 'Latest' does not exist in the document!");
                            }
                        }
                        else
                        {
                            Debug.Log("Document does not exist!");
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to get document: " + snapshotTask.Exception);
                    }
                });
            }
            else
            {
                Debug.LogError($"Failed to initialize Firebase with {task.Result}");
            }
        });
    }
}
