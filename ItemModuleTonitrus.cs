using BS;

namespace Tonitrus
{
    // This create an item module that can be referenced in the item JSON
    public class ItemModuleTonitrus : ItemModule
    {
        public float NPCshockTime = 5f;
        public float buffDuration = 7f;
        public float cooldownDuration = 5f;

        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<ItemTonitrus>();
        }
    }
}
