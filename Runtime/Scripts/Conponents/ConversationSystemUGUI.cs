using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using Prashalt.Unity.ConversationGraph.Conponents.Base;
using UnityEngine.UI;

namespace Prashalt.Unity.ConversationGraph.Conponents
{
    [RequireComponent(typeof(AudioSource))]
    public class ConversationSystemUGUI : ConversationSystemBase
    {
        [Header("GUI-Text")]
        [SerializeField] private TextMeshProUGUI mainText;
        [SerializeField] private TextMeshProUGUI speaker;
        [Header("GUI-Option")]
        [SerializeField] private GameObject optionObjParent;
        [SerializeField] private GameObject optionPrefab;

        [Header("Parameter")]
        [SerializeField] private int textAnimationSpeed;


        private AudioSource audioSource;
        private bool isOptionSelected = false;

        public void Start()
        {
            audioSource = GetComponent<AudioSource>();
            OnNodeChangeAction += OnNodeChange;
            OnShowOptionsAction += OnShowOptions;
            OnConversationFinishedAction += OnConvasationFinished;
        }

        private async UniTask OnNodeChange(ConversationData data)
        {
            if (data.textList == null || data.textList.Count == 0) return;

            //Update Text
            speaker.text = data.speakerName;

            foreach (var text in data.textList)
            {
                audioSource.Play();

                mainText.maxVisibleCharacters = 0;
                mainText.text = text;

                //�A�j���[�V����
                for (var i = 1; i <= mainText.text.Length; i++)
                {
                    mainText.maxVisibleCharacters = i;
                    await UniTask.Delay(textAnimationSpeed);
                    //TODO �N���b�N���Ă���S���ɂ���
                }

                await WaitClick();
                audioSource.Stop();
            }
        }
        private async UniTask OnShowOptions(ConversationData data)
        {
            int id = 0;
            foreach(var option in data.textList)
            {
                var gameObj = Instantiate(optionPrefab, optionObjParent.transform);

                gameObj.GetComponentInChildren<TextMeshProUGUI>().text = option;

                //�l�^�̂͂��Ȃ̂ɁA�V�����ϐ��Ɋi�[���Ă���AddListener���Ȃ��ƂȂ����S�Ēl���Q�ɂȂ�i���Q�ƌ^�݂����ȓ��������B�j
                int optionId = id;
                gameObj.GetComponent<Button>().onClick.AddListener(() => OnSelectOptionButton(optionId));
                id++;
            }
            await UniTask.WaitUntil(() => isOptionSelected);
        }
        private void OnSelectOptionButton(int optionId)
        {
            Debug.Log(optionId);
            foreach (Transform button in optionObjParent.transform)
            {
                Destroy(button.gameObject);
            }
            this.optionId = optionId;

            isOptionSelected = true;
        }
        private void OnConvasationFinished()
        {
            speaker.text = "";
            mainText.text = "";
        }
    }

}