using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using System;

public class FirestoreGetDocumentField : MonoBehaviour
{
    void Start()
    {
        // Firebaseの依存関係をチェックして初期化
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
                Debug.Log("Firebase Firestore initialized successfully.");

                // 特定のドキュメントからデータを取得
                DocumentReference docRef = db.Collection("Users").Document("Tokyo");
                docRef.GetSnapshotAsync().ContinueWithOnMainThread(snapshotTask =>
                {
                    if (snapshotTask.IsCompleted)
                    {
                        DocumentSnapshot snapshot = snapshotTask.Result;
                        if (snapshot.Exists)
                        {
                            Debug.Log($"Document data for {snapshot.Id} document:");

                            // 特定のフィールドの値を取得
                            if (snapshot.TryGetValue("1", out string shinjukuValue))
                            {
                                Debug.Log($"Shinjuku: {shinjukuValue}");
                            }
                            else
                            {
                                Debug.Log("Field 'Shinjuku' does not exist in the document!");
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
