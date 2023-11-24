using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyMove : MonoBehaviour
{

    //�@��]�X�s�[�h
    [SerializeField]
    private float rotateSpeed = 0.5f;
    //�@�X�J�C�{�b�N�X�̃}�e���A��
    private Material skyboxMaterial;


    // Start is called before the first frame update
    void Start()
    {
        //�@Lighting Settings�Ŏw�肵���X�J�C�{�b�N�X�̃}�e���A�����擾
        skyboxMaterial = RenderSettings.skybox;
    }

    // Update is called once per frame
    void Update()
    {
        //�@�X�J�C�{�b�N�X�}�e���A����Rotation�𑀍삵�Ċp�x��ω�������
        skyboxMaterial.SetFloat("_Rotation", Mathf.Repeat(skyboxMaterial.GetFloat("_Rotation") + rotateSpeed * Time.deltaTime, 360f));
    

    }
}
