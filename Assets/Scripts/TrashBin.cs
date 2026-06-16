using UnityEngine;
public class TrashBin : MonoBehaviour {
    public int targetBinType; // 0:可燃, 1:プラ, 2:缶・ペットボトル
    public ParticleSystem correctEffect;
    public ParticleSystem wrongEffect;
    public GameObject correctUI;
    public GameObject wrongUI;

    public void PlayEffect(bool isCorrect) {
        if (isCorrect) {
            if(correctEffect) correctEffect.Play();
            // UIのポップ処理（数秒後に消すなど）をここに
        } else {
            if(wrongEffect) wrongEffect.Play();
        }
    }
}