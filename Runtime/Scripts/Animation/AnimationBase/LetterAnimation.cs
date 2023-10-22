using MagicTween;
using TMPro;

namespace Prashalt.Unity.ConversationGraph.Animation
{
	public abstract class LetterAnimation : ConversationAnimation
	{
		public TextMeshProUGUI TextMeshPro { get; private set; }
		//�L���b�V�����o����悤��Animation�v���p�e�B������Ă���
		public string AnimationId
		{
			get
			{
				//�������ύX����Ă���������̓A�j���[�V��������������Ă��Ȃ������琶����ɓn���B
				if (letterCount != TextMeshPro.GetCharCount() || !isAnimationInit)
				{
					SetAnimation();
				}
				return GetType().Name;
			}
		}
		private bool isAnimationInit;
		protected int letterCount;

		protected LetterAnimation(TextMeshProUGUI textMeshPro)
		{
			TextMeshPro = textMeshPro;
		}

		protected abstract Tween GenerateAnimation(int letterIndex);

		private void SetAnimation()
		{
			isAnimationInit = true;
			for(var i = 0; i < TextMeshPro.GetCharCount(); i++)
			{
				GenerateAnimation(i).SetId(GetType().Name);
			}
		}
	}
}
