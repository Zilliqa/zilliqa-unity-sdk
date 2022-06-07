using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zilliqa.Core;
using Zilliqa.Marketplace;
using Zilliqa.Requests;

public class NFTGridItem : MonoBehaviour
{
    public TMP_Text TokenName;
    public TMP_Text SellerAddress;
    public Image TokenImage;
    public Button TransactButton;
    public Button SecondaryButton;

    public void Initialize(OrderInfo order, string buttonLabel, Action<OrderInfo, Action> onTransactButtonClicked = null)
    {
        Initialize(order, onTransactButtonClicked);
        TransactButton.GetComponentInChildren<TMP_Text>().text = buttonLabel;
    }

    public void Initialize(OrderInfo order, Action<OrderInfo, Action> onTransactButtonClicked = null)
    {
        TokenImage.sprite = order.tokenImage;
        TokenName.text = order.tokenInfo.name;
        SellerAddress.text = order.seller;
        TokenName.text = order.tokenId;

        TransactButton.GetComponentInChildren<TMP_Text>().text = order.type == OrderType.BuyOrder ? "Sell" : "Buy";

        TransactButton.onClick.AddListener(() => onTransactButtonClicked?.Invoke(order, ()=> { }));
    }

    public void Initialize(NFTAsset asset, Sprite Image, Action<NFTAsset> onTransactButtonClicked = null)
    {
        TokenImage.sprite = Image;
        TokenName.text = asset.tokenId;
        SellerAddress.text = asset.minterAddress;

        TransactButton.GetComponentInChildren<TMP_Text>().text = "Sell";

        TransactButton.onClick.AddListener(() => onTransactButtonClicked?.Invoke(asset));
    }

    public void EnableSecondaryButton(NFTAsset asset, string buttonText, Action<NFTAsset> onSecondaryButtonClicked = null)
    {
        SecondaryButton.GetComponentInChildren<TMP_Text>().text = buttonText;
        SecondaryButton.gameObject.SetActive(true);

        SecondaryButton.onClick.AddListener(() => onSecondaryButtonClicked?.Invoke(asset));
    }



}
