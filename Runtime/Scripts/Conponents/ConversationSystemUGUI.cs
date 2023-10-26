using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using Prashalt.Unity.ConversationGraph.Conponents.Base;
using UnityEngine.UI;
using Prashalt.Unity.ConversationGraph.Animation;
using MagicTween;
using System.Collections.Generic;

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
        private bool isSkipText;
        private bool isStartAnimation = false;

        protected override void Start()
        {
            audioSource = GetComponent<AudioSource>();
            OnNodeChangeEvent += OnNodeChange;
            OnShowOptionsEvent += OnShowOptions;
            OnConversationFinishedEvent += OnConvasationFinished;
            OnStartNodeEvent += OnStartNode;

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
        private void OnStartNode(ConversationData data)
        {
			letterAnimation = GetAnimation(data.animation.name, mainText);

			//animation�̃v���p�e�B��o�^
			var intIndex = 0;
			var floatIndex = 0;

			foreach (var info in letterAnimation.GetType().GetFields())
			{
				if (info.FieldType == typeof(int))
				{
					info.SetValue(letterAnimation, data.animation.intProperties[intIndex]);
					intIndex++;
				}
				else if (info.FieldType == typeof(float))
				{
					info.SetValue(letterAnimation, data.animation.floatProperties[floatIndex]);
					floatIndex++;
				}
			}
		}
        private async UniTask OnNodeChange(ConversationData data)
        {
            isSkipText = false;
			if (data.textList == null || data.textList.Count == 0) return;

            var speakerName = ReflectProperty(data.speakerName);

			//Update Text => MagicTween���̃e�L�X�g�X�V����Ȃ��c
			speaker.text = speakerName;

            foreach (var text in data.textList)
            {
				var reflectPropertyText = ReflectProperty(text);
				audioSource.Play();
				//Update Text => MagicTween���̃e�L�X�g�X�V����Ȃ��c
				mainText.SetText(reflectPropertyText);
                mainText.ForceMeshUpdate();

                if (conversationAsset.settings.shouldTextAnimation)
                {
					if (letterAnimation is not null)
                    {
                        Debug.Log("Animation");

                        //�A�j���[�V���������̕�����̒����Ő���
                        var tweenList = await letterAnimation.SetAnimation();

                        //�A�j���[�V�������Đ�
						isStartAnimation = true;
						await PlayAnimation(tweenList);
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
		private async UniTask PlayAnimation(List<Tween> animations)
		{
			foreach (var animation in animations)
			{
				animation.Play();
			}
			await UniTask.WaitUntil(() => !animations.Exists(x => x.IsPlaying()) || isSkipText);
            mainText.ResetCharTweens();   
		}
	}
}