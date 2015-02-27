using UnityEditor;
using Rotorz.ReorderableList;

[CustomEditor(typeof(bl_Gun))]
public class bl_GunEditor : Editor {

    private SerializedProperty GOList;

    private void OnEnable()
    {
        GOList = serializedObject.FindProperty("OnAmmoLauncher");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        bl_Gun script = (bl_Gun)target;
        bool allowSceneObjects = !EditorUtility.IsPersistent(script);
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("","Gun Info", "box");
        script.typeOfGun = (bl_Gun.weaponType)EditorGUILayout.EnumPopup("Type of Gun ", script.typeOfGun);

        if (script.typeOfGun != bl_Gun.weaponType.Launcher)
        {
            script.typeOfBullet = (bl_Gun.BulletType)EditorGUILayout.EnumPopup("Type of Ammo ", script.typeOfBullet);
        }
        //
        script.GunID = EditorGUILayout.IntField("Gun ID ", script.GunID);
        script.GunName = EditorGUILayout.TextField("Gun Name", script.GunName);
        script.localPlayerName = EditorGUILayout.TextField("Player Name", script.localPlayerName);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
      
        if (script.typeOfGun == bl_Gun.weaponType.Machinegun || script.typeOfGun == bl_Gun.weaponType.Pistol || script.typeOfGun == bl_Gun.weaponType.Sniper)
        {
            EditorGUILayout.LabelField("", "Gun Settings", "box");
            EditorGUILayout.BeginVertical("box");
            script.AimPosition = EditorGUILayout.Vector3Field("Aim Position", script.AimPosition);
            script.useSmooth = EditorGUILayout.Toggle("Use Smooth", script.useSmooth);
            script.AimSmooth = EditorGUILayout.Slider("Aim Smooth", script.AimSmooth, 0.01f, 15f);
            script.AimSway = EditorGUILayout.Slider("Aim Sway", script.AimSway, 0.0f, 10);
            script.AimFog = EditorGUILayout.Slider("Aim Fog", script.AimFog, 0.0f, 179);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            script.bullet = EditorGUILayout.ObjectField("Bullet",script.bullet, typeof(UnityEngine.GameObject), allowSceneObjects) as UnityEngine.GameObject;
            script.shell = EditorGUILayout.ObjectField("Shell", script.shell, typeof(UnityEngine.Rigidbody), allowSceneObjects) as UnityEngine.Rigidbody;
            script.muzzlePoint = EditorGUILayout.ObjectField("Fire Point", script.muzzlePoint, typeof(UnityEngine.Transform), allowSceneObjects) as UnityEngine.Transform;
            script.mountPoint = EditorGUILayout.ObjectField("Mount Point", script.mountPoint, typeof(UnityEngine.Transform), allowSceneObjects) as UnityEngine.Transform;
            script.ejectPoint = EditorGUILayout.ObjectField("Eject Point", script.ejectPoint, typeof(UnityEngine.Transform), allowSceneObjects) as UnityEngine.Transform;
            EditorGUILayout.LabelField("","Fire Effects","box");
            script.muzzleFlash = EditorGUILayout.ObjectField("Muzzle Flash", script.muzzleFlash, typeof(UnityEngine.Renderer), allowSceneObjects) as UnityEngine.Renderer;
            script.lightFlash = EditorGUILayout.ObjectField("Fire Light", script.lightFlash, typeof(UnityEngine.Light), allowSceneObjects) as UnityEngine.Light;
            script.impactEffect = EditorGUILayout.ObjectField("Impact Effect", script.impactEffect, typeof(UnityEngine.GameObject), allowSceneObjects) as UnityEngine.GameObject;
            script.bulletHole = EditorGUILayout.ObjectField("Bullet Hole", script.bulletHole, typeof(UnityEngine.GameObject), allowSceneObjects) as UnityEngine.GameObject;
            EditorGUILayout.LabelField("", "Gun properties", "box");
            script.fireRate = EditorGUILayout.FloatField("Fire Rate", script.fireRate);
            script.damage = EditorGUILayout.FloatField("Damage", script.damage);
            script.range = EditorGUILayout.IntField("Range", script.range);
            script.bulletSpeed = EditorGUILayout.FloatField("Bullet Speed", script.bulletSpeed);
            script.impactForce = EditorGUILayout.IntSlider("Imapact Force", script.impactForce, 0, 30);
            script.maxPenetration = EditorGUILayout.IntSlider("Max Penetration",(int) script.maxPenetration,1,6);
            EditorGUILayout.Space();
            script.ShakeIntense = EditorGUILayout.Slider("Shake Intense", script.ShakeIntense, 0.0f, 2.0f);
            script.ShakeSmooth = EditorGUILayout.FloatField("Shake Smooth", script.ShakeSmooth);
            script.kickBackAmount = EditorGUILayout.FloatField("Kick Back Amount", script.kickBackAmount);
            EditorGUILayout.Space();
            script.reloadTime = EditorGUILayout.FloatField("Reload Time", script.reloadTime);
            script.bulletsPerClip = EditorGUILayout.IntField("Bullets Per Clips", script.bulletsPerClip);
            script.maxNumberOfClips = EditorGUILayout.IntField("Max Clips", script.maxNumberOfClips);
            script.numberOfClips = EditorGUILayout.IntSlider("Clips",script.numberOfClips, 0, script.maxNumberOfClips);
            EditorGUILayout.Space();
            script.baseSpread = EditorGUILayout.FloatField("Base Spread", script.baseSpread);
            script.maxSpread = EditorGUILayout.FloatField("Max Spread", script.maxSpread);
            script.spreadPerSecond = EditorGUILayout.FloatField("Spread Per Seconds", script.spreadPerSecond);
            script.decreaseSpreadPerSec = EditorGUILayout.FloatField("Decrease Spread Per Sec", script.decreaseSpreadPerSec);
            EditorGUILayout.LabelField("", "Audio", "box");
            EditorGUILayout.BeginVertical("box");
            script.FireSound = EditorGUILayout.ObjectField("Fire Sound", script.FireSound, typeof(UnityEngine.AudioClip), allowSceneObjects) as UnityEngine.AudioClip;
            if (script.typeOfGun == bl_Gun.weaponType.Sniper)
            {
                script.delayForSecondFireSound = EditorGUILayout.Slider("Delay Second Fire Sound", script.delayForSecondFireSound, 0.0f, 2.0f);
                script.DelaySource = EditorGUILayout.ObjectField("Second Source", script.DelaySource, typeof(UnityEngine.AudioSource), allowSceneObjects) as UnityEngine.AudioSource;
            }
            script.TakeSound = EditorGUILayout.ObjectField("Take Sound", script.TakeSound, typeof(UnityEngine.AudioClip), allowSceneObjects) as UnityEngine.AudioClip;           
            script.SoundReloadByAnim = EditorGUILayout.Toggle("Sounds Reload By Animation", script.SoundReloadByAnim);
            if (!script.SoundReloadByAnim)
            {
                script.ReloadSound = EditorGUILayout.ObjectField("Reload Begin", script.ReloadSound, typeof(UnityEngine.AudioClip), allowSceneObjects) as UnityEngine.AudioClip;
                script.ReloadSound2 = EditorGUILayout.ObjectField("Reload Middle", script.ReloadSound2, typeof(UnityEngine.AudioClip), allowSceneObjects) as UnityEngine.AudioClip;
                script.ReloadSound3 = EditorGUILayout.ObjectField("Reload End", script.ReloadSound3, typeof(UnityEngine.AudioClip), allowSceneObjects) as UnityEngine.AudioClip;
            }
            EditorGUILayout.EndVertical();
            
        }
        if (script.typeOfGun == bl_Gun.weaponType.Burst)
        {
            EditorGUILayout.LabelField("", "Gun Settings", "box");
            EditorGUILayout.BeginVertical("box");
            script.AimPosition = EditorGUILayout.Vector3Field("Aim Position", script.AimPosition);
            script.useSmooth = EditorGUILayout.Toggle("Use Smooth", script.useSmooth);
            script.AimSmooth = EditorGUILayout.Slider("Aim Smooth", script.AimSmooth, 1.0f, 15f);
            script.AimSway = EditorGUILayout.Slider("Aim Sway", script.AimSway, 0.0f, 10);
            script.AimFog = EditorGUILayout.Slider("Aim Fog", script.AimFog, 0.0f, 179);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            script.bullet = EditorGUILayout.ObjectField("Bullet", script.bullet, typeof(UnityEngine.GameObject), allowSceneObjects) as UnityEngine.GameObject;
            script.shell = EditorGUILayout.ObjectField("Shell", script.shell, typeof(UnityEngine.Rigidbody), allowSceneObjects) as UnityEngine.Rigidbody;
            script.muzzlePoint = EditorGUILayout.ObjectField("Fire Point", script.muzzlePoint, typeof(UnityEngine.Transform), allowSceneObjects) as UnityEngine.Transform;
            script.mountPoint = EditorGUILayout.ObjectField("Mount Point", script.mountPoint, typeof(UnityEngine.Transform), allowSceneObjects) as UnityEngine.Transform;
            script.ejectPoint = EditorGUILayout.ObjectField("Eject Point", script.ejectPoint, typeof(UnityEngine.Transform), allowSceneObjects) as UnityEngine.Transform;
            EditorGUILayout.LabelField("", "Fire Effects", "box");
            script.muzzleFlash = EditorGUILayout.ObjectField("Muzzle Flash", script.muzzleFlash, typeof(UnityEngine.Renderer), allowSceneObjects) as UnityEngine.Renderer;
            script.lightFlash = EditorGUILayout.ObjectField("Fire Light", script.lightFlash, typeof(UnityEngine.Light), allowSceneObjects) as UnityEngine.Light;
            script.impactEffect = EditorGUILayout.ObjectField("Impact Effect", script.impactEffect, typeof(UnityEngine.GameObject), allowSceneObjects) as UnityEngine.GameObject;
            script.bulletHole = EditorGUILayout.ObjectField("Bullet Hole", script.bulletHole, typeof(UnityEngine.GameObject), allowSceneObjects) as UnityEngine.GameObject;
            EditorGUILayout.LabelField("", "Gun properties", "box");
            script.fireRate = EditorGUILayout.FloatField("Fire Rate", script.fireRate);
            script.roundsPerBurst = EditorGUILayout.IntSlider("Rounds Per Burst", script.roundsPerBurst, 1, 10);
            script.lagBetweenShots = EditorGUILayout.Slider("Lag Between Shots", script.lagBetweenShots, 0.01f, 5.0f);
            script.damage = EditorGUILayout.FloatField("Damage", script.damage);
            script.range = EditorGUILayout.IntField("Range", script.range);
            script.bulletSpeed = EditorGUILayout.FloatField("Bullet Speed", script.bulletSpeed);
            script.impactForce = EditorGUILayout.IntField("Impact Force", script.impactForce);
            script.maxPenetration = EditorGUILayout.IntSlider("Max Penetration", (int)script.maxPenetration, 1, 6);
            EditorGUILayout.Space();
            script.ShakeIntense = EditorGUILayout.Slider("Shake Intense", script.ShakeIntense, 0.0f, 2.0f);
            script.ShakeSmooth = EditorGUILayout.FloatField("Shake Smooth", script.ShakeSmooth);
            script.kickBackAmount = EditorGUILayout.FloatField("Kick Back Amount", script.kickBackAmount);
            EditorGUILayout.Space();
            script.reloadTime = EditorGUILayout.FloatField("Reload Time", script.reloadTime);
            script.bulletsPerClip = EditorGUILayout.IntField("Bullets Per Clips", script.bulletsPerClip);
            script.maxNumberOfClips = EditorGUILayout.IntField("Max Clips", script.maxNumberOfClips);
            script.numberOfClips = EditorGUILayout.IntSlider("Clips", script.numberOfClips, 0, script.maxNumberOfClips);
            EditorGUILayout.Space();
            script.baseSpread = EditorGUILayout.FloatField("Base Spread", script.baseSpread);
            script.maxSpread = EditorGUILayout.FloatField("Max Spread", script.maxSpread);
            script.spreadPerSecond = EditorGUILayout.FloatField("Spread Per Seconds", script.spreadPerSecond);
            script.decreaseSpreadPerSec = EditorGUILayout.FloatField("Decrease Spread Per Sec", script.decreaseSpreadPerSec);
            EditorGUILayout.LabelField("", "Audio", "box");
            EditorGUILayout.BeginVertical("box");
            script.FireSound = EditorGUILayout.ObjectField("Fire Sound", script.FireSound, typeof(UnityEngine.AudioClip), allowSceneObjects) as UnityEngine.AudioClip;
            script.TakeSound = EditorGUILayout.ObjectField("Take Sound", script.TakeSound, typeof(UnityEngine.AudioClip), allowSceneObjects) as UnityEngine.AudioClip;
             script.SoundReloadByAnim = EditorGUILayout.Toggle("Sounds Reload By Animation", script.SoundReloadByAnim);
             if (!script.SoundReloadByAnim)
             {
                 script.ReloadSound = EditorGUILayout.ObjectField("Reload Begin", script.ReloadSound, typeof(UnityEngine.AudioClip), allowSceneObjects) as UnityEngine.AudioClip;
                 script.ReloadSound2 = EditorGUILayout.ObjectField("Reload Middle", script.ReloadSound2, typeof(UnityEngine.AudioClip), allowSceneObjects) as UnityEngine.AudioClip;
                 script.ReloadSound3 = EditorGUILayout.ObjectField("Reload End", script.ReloadSound3, typeof(UnityEngine.AudioClip), allowSceneObjects) as UnityEngine.AudioClip;
             }
                EditorGUILayout.EndVertical();

        }
        if (script.typeOfGun == bl_Gun.weaponType.Shotgun)
        {
            EditorGUILayout.LabelField("", "ShotGun Settings", "box");
            EditorGUILayout.BeginVertical("box");
            script.AimPosition = EditorGUILayout.Vector3Field("Aim Position", script.AimPosition);
            script.useSmooth = EditorGUILayout.Toggle("Use Smooth", script.useSmooth);
            script.AimSmooth = EditorGUILayout.Slider("Aim Smooth", script.AimSmooth, 1.0f, 15f);
            script.AimSway = EditorGUILayout.Slider("Aim Sway", script.AimSway, 0.0f, 10);
            script.AimFog = EditorGUILayout.Slider("Aim Fog", script.AimFog, 0.0f, 179);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            script.bullet = EditorGUILayout.ObjectField("Bullet", script.bullet, typeof(UnityEngine.GameObject), allowSceneObjects) as UnityEngine.GameObject;
            script.shell = EditorGUILayout.ObjectField("Shell", script.shell, typeof(UnityEngine.Rigidbody), allowSceneObjects) as UnityEngine.Rigidbody;
            script.muzzlePoint = EditorGUILayout.ObjectField("Fire Point", script.muzzlePoint, typeof(UnityEngine.Transform), allowSceneObjects) as UnityEngine.Transform;
            script.mountPoint = EditorGUILayout.ObjectField("Mount Point", script.mountPoint, typeof(UnityEngine.Transform), allowSceneObjects) as UnityEngine.Transform;
            script.ejectPoint = EditorGUILayout.ObjectField("Eject Point", script.ejectPoint, typeof(UnityEngine.Transform), allowSceneObjects) as UnityEngine.Transform;
            EditorGUILayout.LabelField("", "Fire Effects", "box");
            script.muzzleFlash = EditorGUILayout.ObjectField("Muzzle Flash", script.muzzleFlash, typeof(UnityEngine.Renderer), allowSceneObjects) as UnityEngine.Renderer;
            script.lightFlash = EditorGUILayout.ObjectField("Fire Light", script.lightFlash, typeof(UnityEngine.Light), allowSceneObjects) as UnityEngine.Light;
            script.impactEffect = EditorGUILayout.ObjectField("Impact Effect", script.impactEffect, typeof(UnityEngine.GameObject), allowSceneObjects) as UnityEngine.GameObject;
            script.bulletHole = EditorGUILayout.ObjectField("Bullet Hole", script.bulletHole, typeof(UnityEngine.GameObject), allowSceneObjects) as UnityEngine.GameObject;
            EditorGUILayout.LabelField("", "Gun properties", "box");
            script.fireRate = EditorGUILayout.FloatField("Fire Rate", script.fireRate);
            script.damage = EditorGUILayout.FloatField("Damage", script.damage);
            script.pelletsPerShot = EditorGUILayout.IntSlider("Bullets Per Shots", (int)script.pelletsPerShot, 1, 10);
            script.roundsPerTracer = EditorGUILayout.IntSlider("Rounds Per Tracer", (int)script.roundsPerTracer, 1, 5);
            script.range = EditorGUILayout.IntField("Range", script.range);
            script.bulletSpeed = EditorGUILayout.FloatField("Bullet Speed", script.bulletSpeed);
            script.impactForce = EditorGUILayout.IntField("Impact Force", script.impactForce);
            script.maxPenetration = EditorGUILayout.IntSlider("Max Penetration", (int)script.maxPenetration, 1, 6);
            EditorGUILayout.Space();
            script.ShakeIntense = EditorGUILayout.Slider("Shake Intense", script.ShakeIntense, 0.0f, 2.0f);
            script.ShakeSmooth = EditorGUILayout.FloatField("Shake Smooth", script.ShakeSmooth);
            script.kickBackAmount = EditorGUILayout.FloatField("Kick Back Amount", script.kickBackAmount);
            EditorGUILayout.Space();
            script.reloadTime = EditorGUILayout.FloatField("Reload Time", script.reloadTime);
            script.bulletsPerClip = EditorGUILayout.IntField("Bullets Per Clips", script.bulletsPerClip);
            script.maxNumberOfClips = EditorGUILayout.IntField("Max Clips", script.maxNumberOfClips);
            script.numberOfClips = EditorGUILayout.IntSlider("Clips", script.numberOfClips, 0, script.maxNumberOfClips);
            EditorGUILayout.Space();
            script.baseSpread = EditorGUILayout.FloatField("Base Spread", script.baseSpread);
            script.maxSpread = EditorGUILayout.FloatField("Max Spread", script.maxSpread);
            script.spreadPerSecond = EditorGUILayout.FloatField("Spread Per Seconds", script.spreadPerSecond);
            script.decreaseSpreadPerSec = EditorGUILayout.FloatField("Decrease Spread Per Sec", script.decreaseSpreadPerSec);
            EditorGUILayout.LabelField("", "Audio", "box");
            EditorGUILayout.BeginVertical("box");
            script.FireSound = EditorGUILayout.ObjectField("Fire Sound", script.FireSound, typeof(UnityEngine.AudioClip), allowSceneObjects) as UnityEngine.AudioClip;
            script.delayForSecondFireSound = EditorGUILayout.Slider("Delay Second Fire Sound", script.delayForSecondFireSound, 0.0f, 2.0f);
            script.DelaySource = EditorGUILayout.ObjectField("Second Source", script.DelaySource, typeof(UnityEngine.AudioSource), allowSceneObjects) as UnityEngine.AudioSource;
            script.TakeSound = EditorGUILayout.ObjectField("Take Sound", script.TakeSound, typeof(UnityEngine.AudioClip), allowSceneObjects) as UnityEngine.AudioClip;
             script.SoundReloadByAnim = EditorGUILayout.Toggle("Sounds Reload By Animation", script.SoundReloadByAnim);
             if (!script.SoundReloadByAnim)
             {
                 script.ReloadSound = EditorGUILayout.ObjectField("Reload Begin", script.ReloadSound, typeof(UnityEngine.AudioClip), allowSceneObjects) as UnityEngine.AudioClip;
                 script.ReloadSound2 = EditorGUILayout.ObjectField("Reload Middle", script.ReloadSound2, typeof(UnityEngine.AudioClip), allowSceneObjects) as UnityEngine.AudioClip;
                 script.ReloadSound3 = EditorGUILayout.ObjectField("Reload End", script.ReloadSound3, typeof(UnityEngine.AudioClip), allowSceneObjects) as UnityEngine.AudioClip;
             }
                EditorGUILayout.EndVertical();

        }
        if (script.typeOfGun == bl_Gun.weaponType.Launcher)
        {
            EditorGUILayout.LabelField("", "Launcher Settings", "box");
            EditorGUILayout.BeginVertical("box");
            script.AimPosition = EditorGUILayout.Vector3Field("Aim Position", script.AimPosition);
            script.useSmooth = EditorGUILayout.Toggle("Use Smooth", script.useSmooth);
            script.AimSmooth = EditorGUILayout.Slider("Aim Smooth", script.AimSmooth, 1.0f, 15f);
            script.AimSway = EditorGUILayout.Slider("Aim Sway", script.AimSway, 0.0f, 10);
            script.AimFog = EditorGUILayout.Slider("Aim Fog", script.AimFog, 0.0f, 179);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            script.grenade = EditorGUILayout.ObjectField("Grenade", script.grenade, typeof(UnityEngine.GameObject), allowSceneObjects) as UnityEngine.GameObject;
            script.muzzlePoint = EditorGUILayout.ObjectField("Fire Point", script.muzzlePoint, typeof(UnityEngine.Transform), allowSceneObjects) as UnityEngine.Transform;
            EditorGUILayout.LabelField("", "Launcher properties", "box");
            script.fireRate = EditorGUILayout.FloatField("Fire Rate", script.fireRate);
            script.DelayFire = EditorGUILayout.FloatField("Delay Fire", script.DelayFire);
            script.damage = EditorGUILayout.FloatField("Damage", script.damage);
            script.range = EditorGUILayout.IntField("Range", script.range);
            script.bulletSpeed = EditorGUILayout.FloatField("Projectil Speed", script.bulletSpeed);
            script.impactForce = EditorGUILayout.IntField("Impact Force", script.impactForce);
            script.maxPenetration = EditorGUILayout.IntSlider("Max Penetration", (int)script.maxPenetration, 1, 6);
            EditorGUILayout.Space();
            script.ShakeIntense = EditorGUILayout.Slider("Shake Intense", script.ShakeIntense, 0.0f, 2.0f);
            script.ShakeSmooth = EditorGUILayout.FloatField("Shake Smooth", script.ShakeSmooth);
            script.kickBackAmount = EditorGUILayout.FloatField("Kick Back Amount", script.kickBackAmount);
            EditorGUILayout.Space();
            script.reloadTime = EditorGUILayout.FloatField("Reload Time", script.reloadTime);
            script.bulletsPerClip = EditorGUILayout.IntField("Bullets Per Clips", script.bulletsPerClip);
            script.maxNumberOfClips = EditorGUILayout.IntField("Max Clips", script.maxNumberOfClips);
            script.numberOfClips = EditorGUILayout.IntSlider("Clips", script.numberOfClips, 0, script.maxNumberOfClips);
            EditorGUILayout.Space();
            script.baseSpread = EditorGUILayout.FloatField("Base Spread", script.baseSpread);
            script.maxSpread = EditorGUILayout.FloatField("Max Spread", script.maxSpread);
            script.spreadPerSecond = EditorGUILayout.FloatField("Spread Per Seconds", script.spreadPerSecond);
            script.decreaseSpreadPerSec = EditorGUILayout.FloatField("Decrease Spread Per Sec", script.decreaseSpreadPerSec);
            EditorGUILayout.Space();

            ReorderableListGUI.Title("On Not Ammo Disable");
            ReorderableListGUI.ListField(GOList);

            EditorGUILayout.LabelField("", "Audio", "box");
            EditorGUILayout.BeginVertical("box");
            script.FireSound = EditorGUILayout.ObjectField("Fire Sound", script.FireSound, typeof(UnityEngine.AudioClip), allowSceneObjects) as UnityEngine.AudioClip;
            script.TakeSound = EditorGUILayout.ObjectField("Take Sound", script.TakeSound, typeof(UnityEngine.AudioClip), allowSceneObjects) as UnityEngine.AudioClip;
             script.SoundReloadByAnim = EditorGUILayout.Toggle("Sounds Reload By Animation", script.SoundReloadByAnim);
             if (!script.SoundReloadByAnim)
             {
                 script.ReloadSound = EditorGUILayout.ObjectField("Reload Begin", script.ReloadSound, typeof(UnityEngine.AudioClip), allowSceneObjects) as UnityEngine.AudioClip;
                 script.ReloadSound2 = EditorGUILayout.ObjectField("Reload Middle", script.ReloadSound2, typeof(UnityEngine.AudioClip), allowSceneObjects) as UnityEngine.AudioClip;
                 script.ReloadSound3 = EditorGUILayout.ObjectField("Reload End", script.ReloadSound3, typeof(UnityEngine.AudioClip), allowSceneObjects) as UnityEngine.AudioClip;
             }
                 EditorGUILayout.EndVertical();

        }
        EditorGUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
    }

}
