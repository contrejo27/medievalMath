using UnityEngine;

class ExampleGUIAspectsController : MonoBehaviour
{
    private ExperienceSystem exp_bar;
    private int last_level = 1;

    private HealthSystem health_bar;
    private ManaSystem mana_bar;

    private GlobeBarSystem globe_test_bar;

    public Rect HealthBarDimens;
    public bool VerticleHealthBar;
    public Texture HealthBubbleTexture;
    public Texture HealthTexture;
    public float HealthBubbleTextureRotation;

    public void Start()
    {
        health_bar = new HealthSystem(HealthBarDimens, VerticleHealthBar, HealthBubbleTexture, HealthTexture, HealthBubbleTextureRotation);

        health_bar.Initialize();

    }

    public void OnGUI()
    {
        health_bar.DrawBar();
        
    }

	public void updateHealth(int healthLoss){
		health_bar.IncrimentBar(healthLoss * -1);
	}
	
	
    public void Update()
    {
        health_bar.Update();

    }
}