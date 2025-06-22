using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using System;

public class FirestoreGetDocumentField : MonoBehaviour
{
    void Start()
    {
        // Firebase�̈ˑ��֌W���`�F�b�N���ď�����
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
                Debug.Log("Firebase Firestore initialized successfully.");

                // ����̃h�L�������g����f�[�^���擾
                DocumentReference docRef = db.Collection("Users").Document("Tokyo");
                docRef.GetSnapshotAsync().ContinueWithOnMainThread(snapshotTask =>
                {
                    if (snapshotTask.IsCompleted)
                    {
                        DocumentSnapshot snapshot = snapshotTask.Result;
                        if (snapshot.Exists)
                        {
                            Debug.Log($"Document data for {snapshot.Id} document:");

                            // ����̃t�B�[���h�̒l���擾
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
