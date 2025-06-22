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

            text.text = $"�� �ŐV�̃o�[�W����������܂� ��\n�� version {latestVersion} ��\n�� date {date} ��";
            text.enabled = true;

            button.OnClick.AddListener(() => Application.OpenURL(url));
            button.gameObject.SetActive(true);
        }
    }

    void GetLatestVersion() {
        // Firebase�̈ˑ��֌W���`�F�b�N���ď�����

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
                Debug.Log("Firebase Firestore initialized successfully.");

                // ����̃h�L�������g����f�[�^���擾
                DocumentReference docRef = db.Collection("Version").Document("Version");
                docRef.GetSnapshotAsync().ContinueWithOnMainThread(snapshotTask =>
                {
                    if (snapshotTask.IsCompleted)
                    {
                        DocumentSnapshot snapshot = snapshotTask.Result;
                        if (snapshot.Exists)
                        {
                            Debug.Log($"Document data for {snapshot.Id} document:");

                            // ����̃t�B�[���h�̒l���擾
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
