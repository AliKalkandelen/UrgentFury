using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class bl_Customizer : MonoBehaviour {
    /// <summary>
    /// Put here the root of this GO
    /// </summary>
	public Transform target;
    /// <summary>
    /// The name of this weapon or object
    /// </summary>
    public string Go_Name;
    /// <summary>
    /// the speed of rotation of the object
    /// </summary>
	public float speed;
    /// <summary>
    /// speed auto-rotation of the object
    /// </summary>
	public float AutoRotSpeed = -60f;
    /// <summary>
    /// Effect sound when separate parts
    /// </summary>
    public AudioClip SeperateEffect;
    /// <summary>
    /// List of Button
    /// </summary>
	public ListButton Button;
    /// <summary>
    /// List of Parts that can be added to the object
    /// </summary>
	public ListCustomizer Customizer;
    /// <summary>
    /// Containers of parts that can be added
    /// </summary>
	public ListObjVector ObjVector;
    //Private Vars
   private int BarrelID;
   private int OpticsID;
   private int FeederID;
   private int CylinderID;
   private string Level;
   private string CurrentMenu;
   private bool IsSaved = false;
   private bool Show_Warning = false;
   private float rootx;
   private float rooty;
   private bool AutoRot;
   private bool Customize;
   private GUISkin m_Skin;

    /// <summary>
   /// OnEnable use this object to see if it was active
   /// verify whether the manager has the information and if not,
   /// activate the first object in the list
    /// </summary>
    void OnEnable()
    {
        bl_CManager Manager = transform.root.GetComponent<bl_CManager>();
        if (Manager != null)
        {
            m_Skin = Manager.Skin;
            Level = Manager.LevelToLoad;
        }
        else
        {
            Debug.LogError("Manager should be root of this transform"); 
        }
        if (PlayerPrefs.HasKey(Go_Name))
        {
            string cryted = PlayerPrefs.GetString(Go_Name);
            string[] Info = cryted.Split("-"[0]);
            BarrelID = int.Parse(Info[0]);
            OpticsID = int.Parse(Info[1]);
            CylinderID = int.Parse(Info[2]);
            FeederID = int.Parse(Info[3]);
        }
    }

    /// <summary>
    /// Update call one per frame
    /// </summary>
	void Update(){
	ButtonUI();
	RendererModule();

	if(!AutoRot){
	if(Input.GetMouseButton(0)){
	rooty += Input.GetAxis("Mouse Y") * speed;
	rootx += Input.GetAxis("Mouse X") * speed;
	rooty = Mathf.Clamp(rooty,-40,40);
	}
	}else if(AutoRot && !Customize){
	rootx -= Time.deltaTime*AutoRotSpeed;
	if(rooty > 0 || rooty < 0){
	rooty = Mathf.Lerp(rooty,0,0.1f);
	}
	}
	
	target.localEulerAngles = new Vector3(rooty, -rootx, 0);

}

	float vectorx;
	void OnGUI(){

        GUI.skin = m_Skin;
	vectorx = (Screen.width-25)/4;
	if(GUI.Button(new Rect(5,5,vectorx,30),"Back")){
        if (!IsSaved)
        {
            Show_Warning = true;
        }
        else
        {
            Application.LoadLevel(Level);
        }
	}
	if(Customize){
	if(GUI.Button(new Rect(vectorx+5+5,5,vectorx,30),"Unite")){
	Customize=false;
	CurrentMenu = "Menu";
    if (SeperateEffect != null)
    {
        audio.PlayOneShot(SeperateEffect);
    }
	}
	}else{
        if (GUI.Button(new Rect(vectorx + 5 + 5, 5, vectorx, 30), "Separating"))
        {
	Customize=true;
	AutoRot = false;
    if (SeperateEffect != null)
    {
        audio.PlayOneShot(SeperateEffect);
    }
	}
	}
	
	if(Customize){
	if(GUI.Button(new Rect(vectorx+vectorx+5+5+5,5,vectorx,30),"Randomize")){
    BarrelID = Random.Range(0,Customizer.Barrel.Count);
	OpticsID = Random.Range(0,Customizer.Optics.Count);
	FeederID = Random.Range(0,Customizer.Feeder.Count);
	CylinderID = Random.Range(0,Customizer.Cylinder.Count);
	}
	}else{
	if(AutoRot){
	if(GUI.Button(new Rect(vectorx+vectorx+5+5+5,5,vectorx,30),"Auto-rotation 'Off'")){
    AutoRot=false;
	}
	}else{
	if(GUI.Button(new Rect(vectorx+vectorx+5+5+5,5,vectorx,30),"Auto-rotation 'On'")){
    AutoRot=true;
	}
	}
	}

	if(GUI.Button(new Rect(vectorx+vectorx+vectorx+5+5+5+5,5,vectorx,30),"Save")){
        IsSaved = true;
        Save();
	}
    if (GUI.Button(new Rect(Screen.width - 300, Screen.height - 75, 200, 35), "Return"))
    {
        if (!IsSaved)
        {
            Show_Warning = true;
        }
        else
        {
            Application.LoadLevel(Level);
        }
    }
	Menu();

    //if you want to go out and not saved, we notice
    if (Show_Warning)
    {
        GUILayout.BeginArea(new Rect(Screen.width / 2 - 150, Screen.height / 2 , 225, 200), "", "Window");
        GUILayout.Space(5);
        GUILayout.Label("You has not been saved this setting");
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("leave anyway"))
        {
            Application.LoadLevel(Level);
        }
        if (GUILayout.Button("Cancel"))
        {
            Show_Warning = false;
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

	}

	void RendererModule(){
	foreach(infomodule IDBarrel in Customizer.Barrel){
	if(IDBarrel.ID == BarrelID){
	IDBarrel.model.SetActive(true);
	}else{
	IDBarrel.model.SetActive(false);
	}
	}
	
	foreach(infomodule IDOptics in Customizer.Optics){
	if(IDOptics.ID == OpticsID){
	IDOptics.model.SetActive(true);
	}else{
	IDOptics.model.SetActive(false);
	}
	}

	foreach(infomodule IDFeeder in Customizer.Feeder){
	if(IDFeeder.ID == FeederID){
	IDFeeder.model.SetActive(true);
	}else{
	IDFeeder.model.SetActive(false);
	}
	}

	foreach(infomodule IDCylinder in Customizer.Cylinder){
	if(IDCylinder.ID == CylinderID){
	IDCylinder.model.SetActive(true);
	}else{
	IDCylinder.model.SetActive(false);
	}
	}

	}
    /// <summary>
    /// function for save in a PlayerPrefs the code with info
    /// </summary>
    void Save()
    {
        string cryted =  BarrelID + "-" + OpticsID + "-" + CylinderID + "-" + FeederID;
        PlayerPrefs.SetString("Customizer", Go_Name);
        string CurrentCryted = gameObject.name;
        PlayerPrefs.SetString(CurrentCryted, cryted);
        Debug.Log("successfully saved");
    }

    /// <summary>
    /// Show buttons when necessary
    /// </summary>
	void ButtonUI(){
	if(Customize){
	ObjVector.ObjBarrel.transform.localPosition = Vector3.Lerp(ObjVector.ObjBarrel.transform.localPosition,ObjVector.customizerpositionbarrel,0.5f);
	ObjVector.ObjOptics.transform.localPosition = Vector3.Lerp(ObjVector.ObjOptics.transform.localPosition,ObjVector.customizerpositionoptics,0.5f);
	ObjVector.ObjFeeder.transform.localPosition = Vector3.Lerp(ObjVector.ObjFeeder.transform.localPosition,ObjVector.customizerpositionfeeder,0.5f);
	ObjVector.ObjCylinder.transform.localPosition = Vector3.Lerp(ObjVector.ObjCylinder.transform.localPosition,ObjVector.customizerpositioncylinder,0.5f);
	
	Button.Barrel.renderer.enabled = true;
    Button.Optics.renderer.enabled = true;
	Button.Feeder.renderer.enabled = true;
	Button.Cylinder.renderer.enabled = true;
	}else{
	ObjVector.ObjBarrel.transform.localPosition = Vector3.Lerp(ObjVector.ObjBarrel.transform.localPosition,ObjVector.normalpositionbarrel,0.5f);
	ObjVector.ObjOptics.transform.localPosition = Vector3.Lerp(ObjVector.ObjOptics.transform.localPosition,ObjVector.normalpositionoptics,0.5f);
    ObjVector.ObjFeeder.transform.localPosition = Vector3.Lerp(ObjVector.ObjFeeder.transform.localPosition,ObjVector.normalpositionfeeder,0.5f);
	ObjVector.ObjCylinder.transform.localPosition = Vector3.Lerp(ObjVector.ObjCylinder.transform.localPosition,ObjVector.normalpositioncylinder,0.5f);
	

	Button.Barrel.renderer.enabled = false;
	Button.Optics.renderer.enabled = false;
	Button.Feeder.renderer.enabled = false;
	Button.Cylinder.renderer.enabled = false;
	}

	Button.Barrel.transform.LookAt(Camera.main.transform);
	Button.Optics.transform.LookAt(Camera.main.transform);
	Button.Feeder.transform.LookAt(Camera.main.transform);
	Button.Cylinder.transform.LookAt(Camera.main.transform);

	if(Customize){
	Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	RaycastHit hit;

	if (Physics.Raycast(ray,out hit,Mathf.Infinity)){

	if(Button.ActiveButton != null){
	Button.ActiveButton.transform.localScale = new Vector3(0.05f,0.05f,0);
	Button.ActiveButton.FindChild("Text").renderer.enabled = false;
	}
	Button.ActiveButton = hit.transform;
	Button.ActiveButton.transform.localScale = new Vector3(0.08f,0.08f,0);
	Button.ActiveButton.FindChild("Text").renderer.enabled = true;
	}else{
	if(Button.ActiveButton != null){
	Button.ActiveButton.transform.localScale = new Vector3(0.05f,0.05f,0);
	Button.ActiveButton.FindChild("Text").renderer.enabled = false;
	}
	Button.ActiveButton = null;
	}

	}
	}

	void Menu(){
	if(Customize && Button.ActiveButton != null){
	if(Button.ActiveButton.transform.name == "Barrel" && Input.GetMouseButtonDown(0)){
    CurrentMenu = "Barrel";
	}
	if(Button.ActiveButton.transform.name == "Optics" && Input.GetMouseButtonDown(0)){
	CurrentMenu = "Optics";
	}
	if(Button.ActiveButton.transform.name == "Feeder" && Input.GetMouseButtonDown(0)){
	CurrentMenu = "Feeder";
	}
	if(Button.ActiveButton.transform.name == "Cylinder" && Input.GetMouseButtonDown(0)){
	CurrentMenu = "Cylinder";
	}
	}

    if(CurrentMenu == "Barrel"){
	int y = 0;
	GUI.Box (new Rect(5,65,vectorx/2,25),"Barrel");
	GUI.BeginGroup(new Rect(5,95,vectorx/2+2,30*Customizer.Barrel.Count));
	foreach(infomodule ib in Customizer.Barrel){
	if(GUI.Button(new Rect(0,30*y,vectorx/2,25),ib.name)){
    BarrelID = ib.ID;
	}
	y++;
	}
	GUI.EndGroup();
	if(GUI.Button(new Rect(5,120+30*Customizer.Barrel.Count,vectorx/2,20),"Close")){
	CurrentMenu = "Menu";
	}
	}
	
	if(CurrentMenu == "Optics"){
	int y = 0;
	GUI.Box (new Rect(5,65,vectorx/2,25),"Optics");
	GUI.BeginGroup(new Rect(5,95,vectorx/2+2,30*Customizer.Optics.Count));
	foreach(infomodule io in Customizer.Optics){
	if(GUI.Button(new Rect(0,30*y,vectorx/2,25),io.name)){
    OpticsID = io.ID;
	}
	y++;
	}
	GUI.EndGroup();
	if(GUI.Button(new Rect(5,120+30*Customizer.Barrel.Count,vectorx/2,20),"Close")){
	CurrentMenu = "Menu";
	}
	}

	if(CurrentMenu == "Feeder"){
	int y = 0;
	GUI.Box (new Rect(5,65,vectorx/2,25),"Feeder");
	GUI.BeginGroup(new Rect(5,95,vectorx/2+2,30*Customizer.Feeder.Count));
	foreach(infomodule ife in Customizer.Feeder){
	if(GUI.Button(new Rect(0,30*y,vectorx/2,25),ife.name)){
    FeederID = ife.ID;
	}
	y++;
	}
	GUI.EndGroup();
	if(GUI.Button(new Rect(5,120+30*Customizer.Barrel.Count,vectorx/2,20),"Close")){
	CurrentMenu = "Menu";
	}
	}

	if(CurrentMenu == "Cylinder"){
	int y = 0;
	GUI.Box (new Rect(5,65,vectorx/2,25),"Cylinder");
	GUI.BeginGroup(new Rect(5,95,vectorx/2+2,30*Customizer.Cylinder.Count));
	foreach(infomodule ic in Customizer.Cylinder){
	if(GUI.Button(new Rect(0,30*y,vectorx/2,25),ic.name)){
    CylinderID = ic.ID;
	}
	y++;
	}
	GUI.EndGroup();
	if(GUI.Button(new Rect(5,120+30*Customizer.Barrel.Count,vectorx/2,20),"Close")){
	CurrentMenu = "Menu";
	}
	}

	}
	
}




[System.Serializable]
public class ListCustomizer{
public List<infomodule> Barrel = new List<infomodule>();
public List<infomodule> Optics = new List<infomodule>();
public List<infomodule> Feeder = new List<infomodule>();
public List<infomodule> Cylinder = new List<infomodule>();
}

[System.Serializable]
public class ListObjVector{
public GameObject ObjBarrel;
public GameObject ObjOptics;
public GameObject ObjFeeder;
public GameObject ObjCylinder;

public Vector3 normalpositionbarrel;
public Vector3 customizerpositionbarrel;
public Vector3 normalpositionoptics;
public Vector3 customizerpositionoptics;
public Vector3 normalpositionfeeder;
public Vector3 customizerpositionfeeder;
public Vector3 normalpositioncylinder;
public Vector3 customizerpositioncylinder;
}

[System.Serializable]
public class infomodule{
public string name;
public string info;
public int ID;
public GameObject model;
}

[System.Serializable]
public class ListButton{
public Transform ActiveButton;
public float normalButton;
public float selectButton;
public Transform Barrel;
public Transform Optics;
public Transform Feeder;
public Transform Cylinder;
}