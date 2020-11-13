package crc64f84f70ee2cf5bc6a;


public class BarcodeReaderManager
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.honeywell.aidc.AidcManager.CreatedCallback
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreated:(Lcom/honeywell/aidc/AidcManager;)V:GetOnCreated_Lcom_honeywell_aidc_AidcManager_Handler:Com.Honeywell.Aidc.AidcManager/ICreatedCallbackInvoker, DataCollectionLib\n" +
			"";
		mono.android.Runtime.register ("Honeywell.AIDC.CrossPlatform.BarcodeReaderManager, Honeywell.AIDC.CrossPlatform.BarcodeReader", BarcodeReaderManager.class, __md_methods);
	}


	public BarcodeReaderManager ()
	{
		super ();
		if (getClass () == BarcodeReaderManager.class)
			mono.android.TypeManager.Activate ("Honeywell.AIDC.CrossPlatform.BarcodeReaderManager, Honeywell.AIDC.CrossPlatform.BarcodeReader", "", this, new java.lang.Object[] {  });
	}

	public BarcodeReaderManager (android.content.Context p0)
	{
		super ();
		if (getClass () == BarcodeReaderManager.class)
			mono.android.TypeManager.Activate ("Honeywell.AIDC.CrossPlatform.BarcodeReaderManager, Honeywell.AIDC.CrossPlatform.BarcodeReader", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public void onCreated (com.honeywell.aidc.AidcManager p0)
	{
		n_onCreated (p0);
	}

	private native void n_onCreated (com.honeywell.aidc.AidcManager p0);

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
