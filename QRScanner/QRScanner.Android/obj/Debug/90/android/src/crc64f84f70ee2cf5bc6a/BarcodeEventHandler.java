package crc64f84f70ee2cf5bc6a;


public class BarcodeEventHandler
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.honeywell.aidc.BarcodeReader.BarcodeListener,
		java.util.EventListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onBarcodeEvent:(Lcom/honeywell/aidc/BarcodeReadEvent;)V:GetOnBarcodeEvent_Lcom_honeywell_aidc_BarcodeReadEvent_Handler:Com.Honeywell.Aidc.BarcodeReader/IBarcodeListenerInvoker, DataCollectionLib\n" +
			"n_onFailureEvent:(Lcom/honeywell/aidc/BarcodeFailureEvent;)V:GetOnFailureEvent_Lcom_honeywell_aidc_BarcodeFailureEvent_Handler:Com.Honeywell.Aidc.BarcodeReader/IBarcodeListenerInvoker, DataCollectionLib\n" +
			"";
		mono.android.Runtime.register ("Honeywell.AIDC.CrossPlatform.BarcodeEventHandler, Honeywell.AIDC.CrossPlatform.BarcodeReader", BarcodeEventHandler.class, __md_methods);
	}


	public BarcodeEventHandler ()
	{
		super ();
		if (getClass () == BarcodeEventHandler.class)
			mono.android.TypeManager.Activate ("Honeywell.AIDC.CrossPlatform.BarcodeEventHandler, Honeywell.AIDC.CrossPlatform.BarcodeReader", "", this, new java.lang.Object[] {  });
	}


	public void onBarcodeEvent (com.honeywell.aidc.BarcodeReadEvent p0)
	{
		n_onBarcodeEvent (p0);
	}

	private native void n_onBarcodeEvent (com.honeywell.aidc.BarcodeReadEvent p0);


	public void onFailureEvent (com.honeywell.aidc.BarcodeFailureEvent p0)
	{
		n_onFailureEvent (p0);
	}

	private native void n_onFailureEvent (com.honeywell.aidc.BarcodeFailureEvent p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
