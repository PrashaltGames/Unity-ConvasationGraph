using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using Prashalt.Unity.ConversationGraph.Conponents.Base;
using UnityEngine.UI;
using Prashalt.Unity.ConversationGraph.Animation;
using MagicTween;

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

        private AudioSource audioSource;
        private bool isOptionSelected = false;
        private bool isSkipText = false;
        private bool isStartAnimation = false;

        protected override void Start()
        {
            audioSource = GetComponent<AudioSource>();
            OnNodeChangeEvent += OnNodeChange;
            OnShowOptionsEvent += OnShowOptions;
            OnConversationFinishedEvent += OnConvasationFinished;

            base.Start();
        }

        private void Update()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            if(Input.GetMouseButtonDown(0) && isStartAnimation)
            {
                isSkipText = true;
            }
#elif ENABLE_INPUT_SYSTEM
        
#endif
        }

        private async UniTask OnNodeChange(ConversationData data)
        {
            if (data.textList == null || data.textList.Count == 0) return;

            var speakerName = ReflectProperty(data.speakerName);

			//Update Text
			speaker.text = speakerName;

            foreach (var text in data.textList)
            {
				var reflectPropertyText = ReflectProperty(text);
				audioSource.Play();
                mainText.text = reflectPropertyText;

                if (conversationAsset.settings.shouldTextAnimation)
                {
                    isSkipText = false;

                    if(data.animationName != "")
                    {
						var animationId = new LetterFadeInAnimation(mainText).AnimationId;
                        Tween.CompleteAndKillAll(1);
					}
                    else
                    {
						mainText.maxVisibleCharacters = 0;
						//�A�j���[�V����
						for (var i = 1; i <= mainText.text.Length; i++)
						{
							mainText.maxVisibleCharacters = i;
							await UniTask.Delay(conversationAsset.settings.animationSpeed);

							//�N���b�N���Ă���S���ɂ���
							if (isSkipText)
							{
								mainText.maxVisibleCharacters = mainText.text.Length;
								isSkipText = false;
								break;
							}
							else
							{
								isStartAnimation = true;
							}
						}
					}

                    isStartAnimation = false;
                }
                else
                {
                    mainText.maxVisibleCharacters = mainText.text.Length;
                }
                
                if(conversationAsset.settings.isNeedClick)
                {
                    await WaitClick();
                }
                else
                {
                    await UniTask.Delay(conversationAsset.settings.switchingSpeed);
                }
                audioSource.Stop();
            }
        }
        protected async UniTask OnShowOptions(ConversationData data)
        {
            int id = 0;
            isOptionSelected = false;
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
        protected void OnSelectOptionButton(int optionId)
        {
            foreach (Transform button in optionObjParent.transform)
            {
                Destroy(button.gameObject);
            }
            this.optionId = optionId;

            isOptionSelected = true;
        }
        protected void OnConvasationFinished()
        {
            speaker.text = "";
            mainText.text = "";
        }
    }

}