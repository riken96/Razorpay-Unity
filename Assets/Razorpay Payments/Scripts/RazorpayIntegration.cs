using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text;

public class RazorpayIntegration : MonoBehaviour
{
    public Text statusText;
    public Button payButton;

    private const string RAZORPAY_API_KEY = "rzp_test_Ukm1aIbtMWn3Du";
    private const string RAZORPAY_SECRET_KEY = "b6JAucamGWQ1f5t35kADNiqT";

    public void Start()
    {
        payButton.onClick.AddListener(InitiatePayment);
    }

    public void InitiatePayment()
    {
        StartCoroutine(MakePaymentRequest());
    }

    IEnumerator MakePaymentRequest()
    {
        string url = "https://api.razorpay.com/v1/orders";
        string orderId = "ORDER_ID"; // Generate a unique order ID for each transaction
        int amount = 100; // Amount in smallest currency unit (e.g., paisa in INR)

        Dictionary<string, object> jsonRequestBody = new Dictionary<string, object>();
        jsonRequestBody.Add("amount", amount);
        jsonRequestBody.Add("currency", "INR");
        jsonRequestBody.Add("receipt", orderId);

        string requestBodyJson = JsonUtility.ToJson(jsonRequestBody);

        byte[] requestBody = System.Text.Encoding.UTF8.GetBytes(requestBodyJson);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(requestBody);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        string auth = System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(RAZORPAY_API_KEY + ":" + RAZORPAY_SECRET_KEY));
        request.SetRequestHeader("Authorization", "Basic " + auth);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
            statusText.text = "Error: " + request.error;

            ShowRazorPayPaymentModel(auth);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            statusText.text = "Payment Initiated!";
        }
    }

    public void ShowRazorPayPaymentModel(string Data)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject plugin = new AndroidJavaObject("com.razorpay.payments.java.PaymentActivity");
        plugin.Call("LetThePaymentBegins", activity, Data);
        print("#### CALLING AndroidJavaObject DONE");
    }
}
