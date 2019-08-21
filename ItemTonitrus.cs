using UnityEngine;
using BS;

namespace Tonitrus
{
    // The item module will add a unity component to the item object. See unity monobehaviour for more information: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    // This component will apply a shock effect on the item when trigger is pressed
    public class ItemTonitrus : MonoBehaviour
    {

        protected Item item;
        protected EffectShock effectShock;
        public ItemModuleTonitrus module;

        public AudioSource chargeSFX;
        public AudioSource cooldownSFX;
        public Creature poorSoul;

        public bool onCooldown;

        public bool wasActive;

        protected void Awake()
        {
            item = this.GetComponent<Item>();
            item.OnHeldActionEvent += OnHeldAction;
            item.OnCollisionEvent += OnTonitrusCollision;

            module = item.data.GetModule<ItemModuleTonitrus>();
            chargeSFX = item.transform.Find("ChargeSFX").GetComponent<AudioSource>();
            cooldownSFX = item.transform.Find("CooldownSFX").GetComponent<AudioSource>();
        }

        void OnTonitrusCollision(ref CollisionStruct collisionInstance)
        {
            if (effectShock.isActive && !onCooldown)
            {
                if (collisionInstance.targetType == CollisionStruct.TargetType.NPC)
                {
                    GameObject otherObject = collisionInstance.targetCollider.gameObject;
                    bool finished = false;
                    while (!finished)
                    {
                        if (otherObject.transform.parent != null)
                        {
                            otherObject = otherObject.transform.parent.gameObject;
                        }
                        else
                        {
                            finished = true;
                        }

                    }

                    if (otherObject.GetComponent<Creature>())
                    {
                        poorSoul = otherObject.GetComponent<Creature>();
                        foreach (EffectData effect in poorSoul.ragdoll.effects)
                        {
                            if (effect is EffectShock)
                            {
                                effect.Play(module.NPCshockTime);
                                break;
                            }
                        }

                        if (poorSoul.body.handLeft.interactor.grabbedHandle)
                        {
                            foreach(EffectData effect in poorSoul.body.handLeft.interactor.grabbedHandle.item.effects)
                            {
                                if(effect is EffectShock)
                                {
                                    effect.Play(module.NPCshockTime);
                                    break;
                                }
                            }
                        }
                        if (poorSoul.body.handRight.interactor.grabbedHandle)
                        {
                            foreach (EffectData effect in poorSoul.body.handRight.interactor.grabbedHandle.item.effects)
                            {
                                if (effect is EffectShock)
                                {
                                    effect.Play(module.NPCshockTime);
                                    break;
                                }
                            }
                        }


                    }

                }else if (collisionInstance.otherInteractiveObject)
                {
                    if(collisionInstance.otherInteractiveObject.effects.Count > 0)
                    {
                        foreach (EffectData effect in collisionInstance.otherInteractiveObject.effects)
                        {
                            if (effect is EffectShock)
                            {
                                effect.Play(module.NPCshockTime);
                                break;
                            }
                        }
                    }
                                        
                    if(collisionInstance.otherInteractiveObject.handlers.Count > 0)
                    {
                        foreach(Interactor handler in collisionInstance.otherInteractiveObject.handlers)
                        {
                            poorSoul = handler.bodyHand.body.creature;
                            foreach (EffectData effect in poorSoul.ragdoll.effects)
                            {
                                if (effect is EffectShock)
                                {
                                    effect.Play(module.NPCshockTime);
                                    break;
                                }
                            }
                        }
                        
                    }
                    
                }
            }
            
        }

        protected void Start()
        {
            foreach (EffectData effectData in item.effects)
            {
                if (effectData is EffectShock)
                {
                    effectShock = effectData as EffectShock;
                    break;
                }
            }
        }

        public void OnHeldAction(Interactor interactor, Handle handle, Interactable.Action action)
        {
            if (action == Interactable.Action.AlternateUseStart && !onCooldown)
            {
                if (effectShock != null)
                {
                    if (effectShock.isActive)
                    {
                        effectShock.Stop();
                        onCooldown = true;
                        Invoke("Cooldown", module.cooldownDuration);
                    }
                    else
                    {
                        effectShock.Play(1, module.buffDuration, module.NPCshockTime);
                        chargeSFX.Play();

                    }
                }
            }
            if (action == Interactable.Action.Ungrab)
            {
                if (effectShock != null)
                {
                    effectShock.Stop();
                    onCooldown = true;
                    Invoke("Cooldown", module.cooldownDuration);

                }
            }
        }

        void Update()
        {
            if (effectShock.isActive)
            {
                wasActive = true;
            }

            if(!effectShock.isActive && wasActive)
            {
                onCooldown = true;
                wasActive = false;
                Invoke("Cooldown", module.cooldownDuration);
            }
        }

        public void Cooldown()
        {
            onCooldown = false;
            CancelInvoke("Cooldown");
            cooldownSFX.Play();

        }
    }
}