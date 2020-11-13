package crc64f84f70ee2cf5bc6a;


public class BarcodeDeviceEventHandler
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.honeywell.aidc.AidcManager.BarcodeDeviceListener,
		java.util.EventListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onBarcodeDeviceConnectionEvent:(Lcom/honeywell/aidc/BarcodeDeviceConnectionEvent;)V:GetOnBarcodeDeviceConnectionEvent_Lcom_honeywell_aidc_BarcodeDeviceConnectionEvent_Handler:Com.Honeywell.Aidc.AidcManager/IBarcodeDeviceListenerInvoker, DataCollectionLib\n" +
			"";
		mono.android.Runtime.register ("Honeywell.AIDC.CrossPlatform.BarcodeDeviceEventHandler, Honeywell.AIDC.CrossPlatform.BarcodeReader", BarcodeDeviceEventHandler.class, __md_methods);
	}


	public BarcodeDeviceEventHandler ()
	{
		super ();
		if (getClass () == BarcodeDeviceEventHandler.class)
			mono.android.TypeManager.Activate ("Honeywell.AIDC.CrossPlatform.BarcodeDeviceEventHandler, Honeywell.AIDC.CrossPlatform.BarcodeReader", "", this, new java.lang.Object[] {  });
	}


	public void onBarcodeDeviceConnectionEvent (com.honeywell.aidc.BarcodeDeviceConnectionEvent p0)
	{
		n_onBarcodeDeviceConnectionEvent (p0);
	}

	private native void n_onBarcodeDeviceConnectionEvent (com.honeywell.aidc.BarcodeDeviceConnectionEvent p0);

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
