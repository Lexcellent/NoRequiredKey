using System;
using System.Reflection;
using HarmonyLib;
using ItemStatsSystem;
using UnityEngine;

namespace NoRequiredKey
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private Harmony? _harmony = null;

        protected override void OnAfterSetup()
        {
            Debug.Log("NoRequiredKey模组：OnAfterSetup方法被调用");
            if (_harmony != null)
            {
                Debug.Log("NoRequiredKey模组：已修补 先卸载");
                _harmony.UnpatchAll();
            }

            Debug.Log("NoRequiredKey模组：执行修补");
            _harmony = new Harmony("Lexcellent.NoRequiredKey");
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            Debug.Log("NoRequiredKey模组：修补完成");
        }

        protected override void OnBeforeDeactivate()
        {
            Debug.Log("NoRequiredKey模组：OnBeforeDeactivate方法被调用");
            Debug.Log("NoRequiredKey模组：执行取消修补");
            if (_harmony != null)
            {
                _harmony.UnpatchAll();
            }

            Debug.Log("NoRequiredKey模组：执行取消修补完毕");
        }
    }
    [HarmonyPatch(typeof(InteractableBase))]
    public static class InteractableBasePatch
    {
        [HarmonyPatch("TryGetRequiredItem")]
        [HarmonyPrefix]
        public static bool  PrefixTryGetRequiredItem(InteractableBase __instance,  ref (bool, Item) __result)
        {
            try
            {
                if (__instance.requireItem)
                {
                    // 使用 AccessTools.Property.Getter 获取属性的 Getter 方法
                    var cachedMetaGetter = AccessTools.PropertyGetter(typeof(InteractableBase), "CachedMeta");
                    var itemMetaData = (ItemMetaData)cachedMetaGetter.Invoke(__instance, Array.Empty<object>());
                    if (itemMetaData.Catagory == "Key" || itemMetaData.Catagory == "SpecialKey")
                    {
                        __instance.requireItem = false;
                        __result = (true, (Item) null);
                        return false;
                    }
                }

                return true; 
            }
            catch (Exception e)
            {
                Debug.Log($"NoRequiredKey模组：错误：{e.Message}");
                return true;
            }
        }
    }
}