// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.EventSystems;
// using Battle;
// using UnityEngine.UI;
// using System.Runtime.InteropServices;
// public class CommandMenu : MonoBehaviour
// {
//     private RectTransform rect;
//     private Fade fade;
//     public Actor requestingPlayer;

//     private float activeXposition;
//     private float inactiveXposition;

//     private float currentXposition => rect.anchoredPosition3D.x;

//     private bool isActive = false;
//     private bool inPosition = false;
//     [SerializeField] 
//     private float speed;
//     [SerializeField]
//     private GameObject defaultSelection;

//     [SerializeField]
//     private GameObject specialButton;
//     private GameObject lightBoost;
//     // [SerializeField]
//     // private GameObject elementSelectionMenu;
//     // private bool isElementSelectionActive = false;
//     private AudioManager menuPick;
//     private GameObject lastSelection;
//     public int boost = 0;
//     public static CommandMenu instance;    


//     public BattleCommand? selectedCommand {get; private set;} = null;


//     private void Awake() {
//         fade = GetComponent<Fade>();
//         rect = GetComponent<RectTransform>();
//         activeXposition = rect.anchoredPosition3D.x - 900;
//         inactiveXposition = rect.anchoredPosition3D.x;
//         menuPick = FindObjectOfType<AudioManager>();
//         instance = this; 
//     }

//     public void setNullSelectedCommand(){
//         selectedCommand = null;
//     }

//     private void Update()
//     {
//         // if(!inPosition){
//         //     EventSystem.current.SetSelectedGameObject(null);
//         // }else{
//         //     EventSystem.current.SetSelectedGameObject(defaultSelection);
//         // }

        

//         if (isActive && (currentXposition != activeXposition))
//         {
//             fade.FadeIn();
//             float lerpValue = CalculateLerpValue(currentXposition, inactiveXposition, activeXposition);
//             fade.SetAlpha(lerpValue);
//             rect.anchoredPosition3D = Vector3.MoveTowards(
//                 rect.anchoredPosition3D, new Vector3(activeXposition, rect.anchoredPosition3D.y, rect.anchoredPosition3D.z), speed);
            
//         }

//         if(currentXposition == activeXposition){
//             inPosition = true;

//         }else{
//             inPosition = false;
//         }

//         if (!isActive && (currentXposition != inactiveXposition))
//         {
//             // EventSystem.current.SetSelectedGameObject(null);
//             float lerpValue = CalculateLerpValue(currentXposition, activeXposition, inactiveXposition);
//             fade.SetAlpha(lerpValue);
//             rect.anchoredPosition3D = Vector3.MoveTowards(
//                 rect.anchoredPosition3D, new Vector3(inactiveXposition, rect.anchoredPosition3D.y, rect.anchoredPosition3D.z), speed);
//             fade.FadeOut(); 
//         }

//         if(inPosition && Input.GetKeyDown(KeyCode.E)){
//             if(boost < 3 && boost < (boost+requestingPlayer.Stats.BattlePoint)){
//                 if(lightBoost != null){
//                     Destroy(lightBoost);
//                 }
//                 if(boost < 3){
//                     boost+=1;
//                     spawnLightBoost();
                
//                     requestingPlayer.Stats.BattlePoint -=1;
//                     requestingPlayer.GetBattleManager().updateBP();
//                 }
                
//             }
           
//         }

//         if(inPosition && Input.GetKeyDown(KeyCode.Q)){
//             if(boost > 0){
//                 boost-=1;
//                 if(lightBoost != null){
//                     Destroy(lightBoost);
//                 }
//                 spawnLightBoost();
//                 requestingPlayer.Stats.BattlePoint +=1;
//                 requestingPlayer.GetBattleManager().updateBP();
//             }
            
//         }

//         if(inPosition && boost >= 2){
//             specialButton.GetComponent<Button>().interactable = true;
//         }else{
//             specialButton.GetComponent<Button>().interactable = false;
//         }    

//     }

//     public bool outPosition(){
//         if(!isActive && (currentXposition == inactiveXposition)){
//             return true;
//         }else{
//             return false; 
//         }
        
//     }

//     private void spawnLightBoost(){
//         lightBoost = Instantiate(VFXconst.instance.lightEffect, new Vector3(requestingPlayer.transform.position.x, 3.21f, -4f), Quaternion.identity);
//         lightBoost.GetComponent<LightningFade>().flag = true;
//         if(boost == 1){
//             lightBoost.GetComponent<LightningFade>().minIntensity = 50;
//             lightBoost.GetComponent<Light>().color = new Color(209f/255, 39f/255f, 81f/255f);
//             lightBoost.GetComponent<Light>().intensity = 1000;
//             menuPick.PlayAudio(19);
//         }
//         else if(boost == 2){
//             lightBoost.transform.position += new Vector3(0f, 2.5f - lightBoost.transform.position.y, 0f);
//             lightBoost.GetComponent<LightningFade>().minIntensity = 150;
//             lightBoost.GetComponent<Light>().color = new Color(33f/255f, 95f/255f, 255f/255f);
//             lightBoost.GetComponent<Light>().intensity = 1300;
//             menuPick.PlayAudio(21);
//         }
//         else if(boost == 3){
//             lightBoost.transform.position += new Vector3(0f, 3.8f - lightBoost.transform.position.y, 0f);
//             lightBoost.GetComponent<LightningFade>().minIntensity = 300;
            
//             lightBoost.GetComponent<Light>().color = new Color(211f/255f, 236f/255f, 68f/255f);
//             lightBoost.GetComponent<Light>().intensity = 1600;
//             menuPick.PlayAudio(23);
//         }
//     }

//     public void destroyLight(){
//         if(lightBoost != null){
//             Destroy(lightBoost);
//         }
//     }

//     private void playBoostAudio(){
//         if(boost == 1){
            
//         }
//         else if(boost == 2){
            
//         }
//         else if(boost == 3){
            
//         }
//     }

//     private float CalculateLerpValue(float currentValue, float minValue, float maxValue)
//     {
//         return Mathf.Clamp01((currentValue - minValue) / (maxValue - minValue));
//     }

//     public void Activate(int party){
//         if(party == 1){
//             activeXposition = rect.anchoredPosition3D.x - 1400;
//         }else{
//             activeXposition = rect.anchoredPosition3D.x - 900;
//         }
        
//         isActive = true;
//         EventSystem.current.SetSelectedGameObject(defaultSelection);
//         lastSelection = defaultSelection;
//     }

//     public void deActivate(){
//         isActive = false;
        
//     }
//     public void resetCommand(){
//         selectedCommand = null;
//     }

//     public void Attack(){
//         menuPick.PlayAudio(3);
//         selectedCommand = BattleCommand.Attack;
        
//     }
//     public void Heal(){
//         menuPick.PlayAudio(3);
//         selectedCommand = BattleCommand.Heal;
       
//     }
//     public void Special(){
//         menuPick.PlayAudio(3);
//         selectedCommand = BattleCommand.Special;
        
//     }
//     public void Defend(){
//         menuPick.PlayAudio(3);
//         selectedCommand = BattleCommand.Defend;
        
//     }

    
// }
