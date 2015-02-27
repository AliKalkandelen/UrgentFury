using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bl_ReadCustom : MonoBehaviour {

    public string Go_Name;
    protected string m_Read = string.Empty;
    public ListCustomizer Customizer;
    protected int BarrelID;
    protected int OpticID;
    protected int CylinderID;
    protected int FeederID;

    void OnEnable()
    {
        Read();
    }

    void Read()
    {
        if (Go_Name == string.Empty || Go_Name == null)
        {
            Go_Name = gameObject.name;
        }
        if (PlayerPrefs.HasKey(Go_Name))
        {
            m_Read = PlayerPrefs.GetString(Go_Name);
            string[] descompile = m_Read.Split("-"[0]);
                BarrelID = int.Parse(descompile[0]);
                OpticID = int.Parse(descompile[1]);
                CylinderID = int.Parse(descompile[2]);
                FeederID = int.Parse(descompile[3]);
                RendererModule();          
        }
    }

    void FixedUpdate()
    {
        RendererModule();
    }

    void RendererModule()
    {
        foreach (infomodule IDBarrel in Customizer.Barrel)
        {
            if (IDBarrel.ID == BarrelID)
            {
                IDBarrel.model.SetActive(true);
            }
            else
            {
                IDBarrel.model.SetActive(false);
            }
        }

        foreach (infomodule IDOptics in Customizer.Optics)
        {
            if (IDOptics.ID == OpticID)
            {
                IDOptics.model.SetActive(true);
            }
            else
            {
                IDOptics.model.SetActive(false);
            }
        }

        foreach (infomodule IDFeeder in Customizer.Feeder)
        {
            if (IDFeeder.ID == FeederID)
            {
                IDFeeder.model.SetActive(true);
            }
            else
            {
                IDFeeder.model.SetActive(false);
            }
        }

        foreach (infomodule IDCylinder in Customizer.Cylinder)
        {
            if (IDCylinder.ID == CylinderID)
            {
                IDCylinder.model.SetActive(true);
            }
            else
            {
                IDCylinder.model.SetActive(false);
            }
        }

    }

    [System.Serializable]
    public class infomodule
    {
        public int ID;
        public GameObject model;
    }

    [System.Serializable]
    public class ListCustomizer
    {
        public List<infomodule> Barrel = new List<infomodule>();
        public List<infomodule> Optics = new List<infomodule>();
        public List<infomodule> Feeder = new List<infomodule>();
        public List<infomodule> Cylinder = new List<infomodule>();
    }
}
