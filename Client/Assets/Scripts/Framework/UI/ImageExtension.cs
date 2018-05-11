using System;
using UnityEngine;
using UnityEngine.UI;

public static class ImageExtension
{

    public static void SetSprite(this Image image, GameObject prefab, bool isShowSpecMatrial = false)
    {
        UIUtility.SetImageSprite(image, prefab, isShowSpecMatrial);
    }

    public static void SetSprite(this Image image, string prefabPath, UIFormScript formScript, bool loadSync = true, bool needCached = false, bool unloadBelongedAssetBundleAfterLoaded = false, bool isShowSpecMatrial = false)
    {
        UIUtility.SetImageSprite(image, prefabPath, formScript, loadSync, needCached, unloadBelongedAssetBundleAfterLoaded, isShowSpecMatrial);
    }

    public static void SetSprite(this Image image, Image targetImage)
    {
        UIUtility.SetImageSprite(image, targetImage);
    }

    //public static void SetSprite(this Image image, Sprite sprite, ImageAlphaTexLayout imageAlphaTexLayout)
    //{
    //    if (image is Image2)
    //    {
    //        (image as Image2).alphaTexLayout = imageAlphaTexLayout;
    //    }
    //    image.sprite = sprite;
    //}

    public static void CustomFillAmount(this Image image, float value)
    {
        if (image != null && image.fillAmount != value)
        {
            image.fillAmount = value;
        }
    }
}
