using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"DOTween.dll",
		"Google.Protobuf.dll",
		"Newtonsoft.Json.dll",
		"System.Core.dll",
		"System.dll",
		"UniTask.dll",
		"Unity.Burst.dll",
		"Unity.Collections.dll",
		"Unity.Entities.dll",
		"UnityEngine.AndroidJNIModule.dll",
		"UnityEngine.CoreModule.dll",
		"UnityEngine.JSONSerializeModule.dll",
		"UnityEngine.UI.dll",
		"YooAsset.dll",
		"mscorlib.dll",
		"protobuf-net.Core.dll",
		"protobuf-net.dll",
		"spine-unity.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_Logic.HotUpdateMain.<GoToUIScene>d__7>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_Logic.HotUpdateMain.<InitTypeAndMetaData>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_Logic.HotUpdateMain.<LoadMetadataForAOTAssemblies>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornSceneHelper.<InitInputPrefab>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<CaptureScreenAsync>d__16,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<CaptureScreenshot>d__18,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<ChangeSoftness>d__13,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<DoMonsterPos>d__33,Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<DoPlayerPos>d__35>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<DownloadFileByUrl>d__135>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<DownloadNotice>d__133>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<EnableGuide>d__36>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<EnableLoading>d__20>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<InitBlur>d__17>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<LoadImage>d__136>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<PlayUIImageSweepFX>d__23,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<PlayUIImageTranstionFX>d__22,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<RefreshAllPanelL10N>d__150>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosB2U>d__4,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosLtoR>d__6,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosRtoL>d__7,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosUtoB>d__5,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndScale>d__8,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndScaleWithFour>d__10,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<SetScaleWithBounce>d__11,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<SetScaleWithBounceClose>d__12,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<SpawnMapElement>d__37>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<SpawnMapElement>d__38>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.UnicornUIHelper.<TypeWriteEffect>d__94>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.JsonManager.<LoadPlayerData>d__10,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.JsonManager.<LoadPlayerData>d__9,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.JsonManager.<LoadSharedData>d__11,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.JsonManager.<SavePlayerData>d__8>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<HotFix_UI.JsonManager.<SaveSharedData>d__12>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.AudioManager.<LoadAudio>d__18>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.DemoEntry.<InitTables>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.DemoEntry.<Loader>d__3,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.ImageComponent.<SetSpriteAsync>d__3<object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.IntroGuide.<InitInputPrefab>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.LoadingScene.<WaitForCompleted>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.ResourcesLoader.<InitAsync>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.ResourcesManager.<InstantiateAsync>d__16,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.ResourcesManager.<InstantiateAsync>d__17,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.ResourcesManager.<InstantiateAsync>d__20,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.ResourcesManager.<InstantiateAsync>d__21,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.ResourcesManager.<LoadAssetAsync>d__10<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.ResourcesManager.<LoadAssetAsync>d__11<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.ResourcesManager.<LoadAssetAsync>d__12,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.RunTimeScene.<InitInputPrefab>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.Scene.<WaitForCompleted>d__14>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.SceneResManager.<UnloadSceneAsync>d__6>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.SceneResManager.<WaitForCompleted>d__7>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UICommon_Label.<InitBag>d__25>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UICommon_Label.<InitEquip>d__26>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UICommon_Label.<InitEquipCompound>d__27>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UICommon_Label.<InitEquipCompoundSelected>d__30>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UICommon_Label.<InitMaterial>d__32>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UICommon_Prompt.<StartAnimation>d__11>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UICommon_Reward.<InitRewardItems>d__16>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIContainerBoxBar.<CreateReward>d__36,float>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIHelper.<CreateAsync>d__10<object,object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIHelper.<CreateAsync>d__12,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIHelper.<CreateAsync>d__13<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIHelper.<CreateAsync>d__14<object,object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIHelper.<CreateAsync>d__15,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIHelper.<CreateAsync>d__16<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIHelper.<CreateAsync>d__17<object,object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIHelper.<CreateAsync>d__8,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIHelper.<CreateAsync>d__9<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIHelper.<CreateAsyncNew>d__6<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIHelper.<CreateAsyncNew>d__7,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIHelper.<CreateAsyncWithPrefabKey>d__11<object,object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIHelper.<CreateOverLayTipsAsync>d__18<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIImageExtensions.<SetSpriteAsync>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIListComponent.<CreateWithKeyAsync>d__44<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIListComponent.<CreateWithKeyAsync>d__45<object,object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIListComponent.<CreateWithKeyAsync>d__46,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIListComponent.<CreateWithUITypeAsync>d__39,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIListComponent.<CreateWithUITypeAsync>d__40<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIListComponent.<CreateWithUITypeAsync>d__41,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIListComponent.<CreateWithUITypeAsync>d__42<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIListComponent.<CreateWithUITypeAsync>d__43<object,object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIListComponent.<InnerCreateWithKeyAsync>d__30<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIListComponent.<InnerCreateWithUITypeAsync>d__28,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIListComponent.<InnerCreateWithUITypeAsync>d__29,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UILoading.<DoFillAmount>d__20>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UILoading.<LoadAssets>d__18>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UILoading.<LoadObjectAsync>d__19>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIManager.<CreateAsync>d__52,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIManager.<CreateAsync>d__53<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIManager.<CreateAsync>d__54<object,object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIManager.<CreateAsync>d__55<object,object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIManager.<CreateAsync>d__56,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIManager.<CreateAsync>d__57<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIManager.<CreateAsync>d__58<object,object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIManager.<CreateAsync>d__59,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIManager.<CreateAsync>d__60<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIManager.<CreateAsync>d__61<object,object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIManager.<CreateAsyncNew>d__50<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIManager.<CreateAsyncNew>d__51,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIManager.<CreateInnerAsync>d__39,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIManager.<CreateInnerAsync>d__40,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIManager.<EnableLoading>d__28>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIManager.<GetGameObjectAsync>d__41,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIManager.<GetGameObjectAsync>d__42,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIManager.<PlayTranstionFX>d__30>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_Achieve.<ClosePanel>d__37>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_Achieve.<CreateGroup>d__46>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_Achieve.<InitNode>d__36>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_Activity_Monopoly.<DoScaleAnim>d__94,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_Activity_Monopoly.<PlayerMoveAnim>d__91>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_Activity_Monopoly.<UpdateGrid>d__96>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_AnimTools.<AlphaRefresh>d__19>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_AnimTools.<ScaleRefresh>d__18>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_AnimTools.<TranRefresh>d__20>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_Challege.<CreateReward>d__85,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_Challege.<UpdateFromCurrentMainID>d__74,int>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_Compound.<Effect1>d__42,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_Compound.<Effect2>d__41,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_Compound.<SetEffect>d__40,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_CompoundDongHua.<GenereteItem>d__28>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_CompoundDongHua.<InitRewardItems>d__35>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_CompoundDongHua.<IsGetData>d__33,byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_CompoundDongHua.<setScaleCanCancel>d__36,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_JiyuGame.<CreateTagPanel>d__46>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_JiyuGame.<InitPanel>d__37>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_JiyuGame.<OnButtonClickAnim>d__74>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_JiyuGame.<SetEaseEffectForTag5>d__73>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_JiyuGame.<UnLockTag>d__40>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_Mail.<ClosePanel>d__40>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_Mail.<InitMailPanel>d__47>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_Main.<IntroGuideOrder>d__92>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_Main.<PlayTreasureAnim>d__107>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_Main.<PlayerTipOccurAsyc>d__101>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_Notice.<ClosePanel>d__30>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_Notice.<DataInit>d__32>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_RunTimeHUD.<CheckEnemyClear>d__71,byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_RunTimeHUD.<CheckQueryExist>d__72,byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_RunTimeHUD.<IntroGuideOrder>d__76>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_RunTimeHUD.<PlaySpineUIFX>d__70>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_RunTimeHUD.<SpawnEnemy>d__79>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UIPanel_Settings.<ClosePanel>d__47>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UISubPanel_Equipment.<Init>d__17>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UISubPanel_Guid.<AlphaRefresh>d__41>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UISubPanel_Guid.<ScaleRefresh>d__40>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UISubPanel_Guid.<TranRefresh>d__42>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UISubPanel_Shop_1403_Fund.<Module1403_Help_CreateOneRowItem>d__53>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.UISubPanel_Shop_Pre.<PreInit>d__23>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.YooResourcesLoader.<InstantiateAsync>d__10,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.YooResourcesLoader.<InstantiateAsync>d__11,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.YooResourcesLoader.<InstantiateAsync>d__12,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.YooResourcesLoader.<InstantiateAsync>d__9,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.YooResourcesLoader.<LoadAssetAsync>d__3<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.YooResourcesLoader.<LoadAssetAsync>d__4,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.YooSceneLoader.<UnloadSceneAsync>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<XFramework.YooSceneLoader.<WaitForCompleted>d__5>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_Logic.HotUpdateMain.<GoToUIScene>d__7>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_Logic.HotUpdateMain.<InitTypeAndMetaData>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_Logic.HotUpdateMain.<LoadMetadataForAOTAssemblies>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornSceneHelper.<InitInputPrefab>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<CaptureScreenAsync>d__16,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<CaptureScreenshot>d__18,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<ChangeSoftness>d__13,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<DoMonsterPos>d__33,Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<DoPlayerPos>d__35>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<DownloadFileByUrl>d__135>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<DownloadNotice>d__133>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<EnableGuide>d__36>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<EnableLoading>d__20>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<InitBlur>d__17>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<LoadImage>d__136>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<PlayUIImageSweepFX>d__23,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<PlayUIImageTranstionFX>d__22,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<RefreshAllPanelL10N>d__150>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosB2U>d__4,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosLtoR>d__6,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosRtoL>d__7,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosUtoB>d__5,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndScale>d__8,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndScaleWithFour>d__10,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<SetScaleWithBounce>d__11,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<SetScaleWithBounceClose>d__12,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<SpawnMapElement>d__37>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<SpawnMapElement>d__38>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.UnicornUIHelper.<TypeWriteEffect>d__94>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.JsonManager.<LoadPlayerData>d__10,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.JsonManager.<LoadPlayerData>d__9,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.JsonManager.<LoadSharedData>d__11,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.JsonManager.<SavePlayerData>d__8>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<HotFix_UI.JsonManager.<SaveSharedData>d__12>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.AudioManager.<LoadAudio>d__18>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.DemoEntry.<InitTables>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.DemoEntry.<Loader>d__3,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.ImageComponent.<SetSpriteAsync>d__3<object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.IntroGuide.<InitInputPrefab>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.LoadingScene.<WaitForCompleted>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.ResourcesLoader.<InitAsync>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.ResourcesManager.<InstantiateAsync>d__16,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.ResourcesManager.<InstantiateAsync>d__17,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.ResourcesManager.<InstantiateAsync>d__20,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.ResourcesManager.<InstantiateAsync>d__21,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.ResourcesManager.<LoadAssetAsync>d__10<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.ResourcesManager.<LoadAssetAsync>d__11<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.ResourcesManager.<LoadAssetAsync>d__12,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.RunTimeScene.<InitInputPrefab>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.Scene.<WaitForCompleted>d__14>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.SceneResManager.<UnloadSceneAsync>d__6>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.SceneResManager.<WaitForCompleted>d__7>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UICommon_Label.<InitBag>d__25>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UICommon_Label.<InitEquip>d__26>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UICommon_Label.<InitEquipCompound>d__27>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UICommon_Label.<InitEquipCompoundSelected>d__30>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UICommon_Label.<InitMaterial>d__32>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UICommon_Prompt.<StartAnimation>d__11>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UICommon_Reward.<InitRewardItems>d__16>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIContainerBoxBar.<CreateReward>d__36,float>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIHelper.<CreateAsync>d__10<object,object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIHelper.<CreateAsync>d__12,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIHelper.<CreateAsync>d__13<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIHelper.<CreateAsync>d__14<object,object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIHelper.<CreateAsync>d__15,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIHelper.<CreateAsync>d__16<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIHelper.<CreateAsync>d__17<object,object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIHelper.<CreateAsync>d__8,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIHelper.<CreateAsync>d__9<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIHelper.<CreateAsyncNew>d__6<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIHelper.<CreateAsyncNew>d__7,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIHelper.<CreateAsyncWithPrefabKey>d__11<object,object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIHelper.<CreateOverLayTipsAsync>d__18<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIImageExtensions.<SetSpriteAsync>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIListComponent.<CreateWithKeyAsync>d__44<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIListComponent.<CreateWithKeyAsync>d__45<object,object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIListComponent.<CreateWithKeyAsync>d__46,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIListComponent.<CreateWithUITypeAsync>d__39,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIListComponent.<CreateWithUITypeAsync>d__40<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIListComponent.<CreateWithUITypeAsync>d__41,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIListComponent.<CreateWithUITypeAsync>d__42<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIListComponent.<CreateWithUITypeAsync>d__43<object,object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIListComponent.<InnerCreateWithKeyAsync>d__30<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIListComponent.<InnerCreateWithUITypeAsync>d__28,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIListComponent.<InnerCreateWithUITypeAsync>d__29,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UILoading.<DoFillAmount>d__20>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UILoading.<LoadAssets>d__18>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UILoading.<LoadObjectAsync>d__19>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIManager.<CreateAsync>d__52,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIManager.<CreateAsync>d__53<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIManager.<CreateAsync>d__54<object,object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIManager.<CreateAsync>d__55<object,object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIManager.<CreateAsync>d__56,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIManager.<CreateAsync>d__57<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIManager.<CreateAsync>d__58<object,object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIManager.<CreateAsync>d__59,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIManager.<CreateAsync>d__60<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIManager.<CreateAsync>d__61<object,object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIManager.<CreateAsyncNew>d__50<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIManager.<CreateAsyncNew>d__51,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIManager.<CreateInnerAsync>d__39,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIManager.<CreateInnerAsync>d__40,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIManager.<EnableLoading>d__28>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIManager.<GetGameObjectAsync>d__41,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIManager.<GetGameObjectAsync>d__42,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIManager.<PlayTranstionFX>d__30>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_Achieve.<ClosePanel>d__37>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_Achieve.<CreateGroup>d__46>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_Achieve.<InitNode>d__36>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_Activity_Monopoly.<DoScaleAnim>d__94,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_Activity_Monopoly.<PlayerMoveAnim>d__91>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_Activity_Monopoly.<UpdateGrid>d__96>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_AnimTools.<AlphaRefresh>d__19>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_AnimTools.<ScaleRefresh>d__18>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_AnimTools.<TranRefresh>d__20>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_Challege.<CreateReward>d__85,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_Challege.<UpdateFromCurrentMainID>d__74,int>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_Compound.<Effect1>d__42,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_Compound.<Effect2>d__41,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_Compound.<SetEffect>d__40,Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_CompoundDongHua.<GenereteItem>d__28>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_CompoundDongHua.<InitRewardItems>d__35>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_CompoundDongHua.<IsGetData>d__33,byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_CompoundDongHua.<setScaleCanCancel>d__36,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_JiyuGame.<CreateTagPanel>d__46>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_JiyuGame.<InitPanel>d__37>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_JiyuGame.<OnButtonClickAnim>d__74>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_JiyuGame.<SetEaseEffectForTag5>d__73>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_JiyuGame.<UnLockTag>d__40>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_Mail.<ClosePanel>d__40>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_Mail.<InitMailPanel>d__47>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_Main.<IntroGuideOrder>d__92>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_Main.<PlayTreasureAnim>d__107>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_Main.<PlayerTipOccurAsyc>d__101>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_Notice.<ClosePanel>d__30>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_Notice.<DataInit>d__32>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_RunTimeHUD.<CheckEnemyClear>d__71,byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_RunTimeHUD.<CheckQueryExist>d__72,byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_RunTimeHUD.<IntroGuideOrder>d__76>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_RunTimeHUD.<PlaySpineUIFX>d__70>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_RunTimeHUD.<SpawnEnemy>d__79>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UIPanel_Settings.<ClosePanel>d__47>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UISubPanel_Equipment.<Init>d__17>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UISubPanel_Guid.<AlphaRefresh>d__41>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UISubPanel_Guid.<ScaleRefresh>d__40>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UISubPanel_Guid.<TranRefresh>d__42>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UISubPanel_Shop_1403_Fund.<Module1403_Help_CreateOneRowItem>d__53>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.UISubPanel_Shop_Pre.<PreInit>d__23>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.YooResourcesLoader.<InstantiateAsync>d__10,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.YooResourcesLoader.<InstantiateAsync>d__11,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.YooResourcesLoader.<InstantiateAsync>d__12,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.YooResourcesLoader.<InstantiateAsync>d__9,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.YooResourcesLoader.<LoadAssetAsync>d__3<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.YooResourcesLoader.<LoadAssetAsync>d__4,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.YooSceneLoader.<UnloadSceneAsync>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<XFramework.YooSceneLoader.<WaitForCompleted>d__5>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<float>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<int>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_Logic.HotUpdateMain.<Start>d__6>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.HybridEventSystem.<OnBossDieEvent>d__5>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.HybridEventSystem.<OnGuide313>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.HybridEventSystem.<OnPlayerDieEvent>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.HybridEventSystem.<SwitchBossScene>d__6>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornSceneHelper.<InitRunTimeScene>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__0>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__1>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__2>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__3>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__4>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__5>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__7>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__8>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__9>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_1.<<SetRewardOnClick>b__10>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__0>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__1>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__2>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__3>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__4>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__5>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__6>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__7>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_1.<<SetRewardOnClickTips>b__8>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<AddReward>d__103>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<AddRewardInternal>d__104>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<AddRewardsInternal>d__105>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<ExitRunTimeScene>d__47>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.UnicornUIHelper.<SetRewardOnClickWithNoBtn>d__100>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<HotFix_UI.NetWorkManager.<AttemptReconnect>d__13>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.Global.<CameraShake>d__32>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.Global.<DoCameraFOV>d__33>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.IntroGuide.<InitRunTimeScene>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.MenuScene.<HandleRedDot>d__31>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.MenuScene.<StandAloneMode>d__20>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.RunTimeScene.<InitRunTimeScene>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UICommonFunButton.<Set3405BankWeb>d__8>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UICommon_EquipTips.<RefreshLevelUp>d__32>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UICommon_Prompt.<AlphaUpdate>d__12>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Achieve_List.<BottomInit>d__21>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Achieve_List.<CreateTask>d__18>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Activity_Challenge.<CreateTasks>d__65>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Activity_Challenge.<DownSelectSet>d__64>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Activity_EnergyShop.<UpdateConatainerItemTopAsync>d__47>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Activity_Monopoly.<SetPlayerPos>d__95>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Activity_NewSign.<CreateOneDaySign>d__41>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_AnimTools.<Refresh>d__23>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Bank.<Anim>d__36>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_BattleInfo.<OnClickBindings>d__66>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_BattleShop.<CreateBindingItem>d__94>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_BattleShop.<CreateSkillsItem>d__79>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_BattleShop.<Guide>d__66>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_BattleShop.<OnClickBindings>d__70>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_BattleTecnology.<Guide>d__32>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_BattleTecnology.<SetTechBtnItem>d__41>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_BattleTecnology.<SetTechItemUI>d__44>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_BuyEnergy.<Anim>d__40>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Challege.<CreateMainTreadAreaInfo>d__80>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Challege.<SetFromCurrentAreaID>d__75>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Collection_Unlock.<PlayAnim>d__26>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Compound.<InitPanel>d__39>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Compound.<OnSelected>d__34>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Compound.<SpawnItems>d__50>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Compound.<SpawnSelectedCompound>d__47>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_CompoundSuc.<Init>d__7>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_EquipDownGrade.<InitPanel>d__22>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_EquipDownGrade.<RefreshLevelUp>d__23>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_EquipTips.<Guide>d__39>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_EquipTips.<RefreshLevelUp>d__43>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Equipment.<Anim>d__45>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Equipment.<InitPanel>d__59>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Equipment.<InitTab2WidegetInfo>d__65>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Equipment.<OnClickBag>d__72>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Equipment.<OnClickEquip>d__71>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_First_Charge.<CreatTime>d__18>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_GuideSkillChoose.<ChoseOnSkillGroup>d__21>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_GuideTips.<Update>d__13>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_JiyuGame.<Guide>d__39>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_JiyuGame.<OnBtnClickEvent>d__72>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_JiyuGame.<ShopModuleDelayRefresh>d__52>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Login.<GetLocationInfoNew>d__10>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Login.<Init>d__7>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Main.<InitNode>d__89>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Main.<OnBuyEnergyBtnClick>d__97>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Main.<UpdateTreasure>d__108>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_MonopolyTaskShop.<InitNode>d__43>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_MonsterCollection.<CreateBottomBtnList>d__30>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_MonsterCollection.<PlayAnim>d__36>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Notice.<CreateNoticeItem>d__36>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Notice.<SetSpriteByLocalPath>d__35>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Pass.<CreateItem1>d__70>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Pass.<CreateItem>d__80>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Rebirth.<SetTxtTime>d__20>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_RunTimeHUD.<SetEnv>d__85>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_SelectBoxNomal.<InitNode>d__28>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Share.<CreateItems>d__33>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Shop.<Module1201_Help_CreateItem>d__114>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Shop.<Module1302_Help_SetGift>d__124>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Shop.<SetModule1201RedPointState>d__87>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Shop.<SetModule1302RedPointStateHelp>d__91>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Sweep.<ChangeMagnification>d__67>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Talent.<Anim>d__50>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Talent_Prop.<InitNode>d__12>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Task_DailyAndWeekly.<BottomInit>d__54>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Task_DailyAndWeekly.<CreateTask>d__47>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UIPanel_Task_DailyAndWeekly.<TopScoreBoxSet>d__58>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UISubPanel_EnergyShopItem.<OnBtnBuyOnClick>d__23>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UISubPanel_EnergyShopItem.<OnSecBtnClick>d__25>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UISubPanel_EnergyShopItem.<SetReward>d__21>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UISubPanel_Guid.<Refresh>d__46>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UISubPanel_IconBtnItem.<Set3405BankWeb>d__9>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UISubPanel_Pass_Token.<CreateItem>d__24>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UISubPanel_Shop_1102_SBox.<InitEffect>d__49>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UISubPanel_Shop_1301_ChapterGift.<CreateItem>d__33>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UISubPanel_Shop_1301_ChapterGift.<ImgSet>d__29>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UISubPanel_Shop_Gift_Item.<InitEffect>d__17>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<XFramework.UISubPanel_Shop_Pre.<CreateReward>d__26>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_Logic.HotUpdateMain.<Start>d__6>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.HybridEventSystem.<OnBossDieEvent>d__5>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.HybridEventSystem.<OnGuide313>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.HybridEventSystem.<OnPlayerDieEvent>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.HybridEventSystem.<SwitchBossScene>d__6>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornSceneHelper.<InitRunTimeScene>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__0>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__1>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__2>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__3>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__4>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__5>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__7>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__8>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__9>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_1.<<SetRewardOnClick>b__10>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__0>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__1>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__2>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__3>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__4>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__5>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__6>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__7>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_1.<<SetRewardOnClickTips>b__8>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<AddReward>d__103>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<AddRewardInternal>d__104>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<AddRewardsInternal>d__105>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<ExitRunTimeScene>d__47>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.UnicornUIHelper.<SetRewardOnClickWithNoBtn>d__100>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<HotFix_UI.NetWorkManager.<AttemptReconnect>d__13>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.Global.<CameraShake>d__32>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.Global.<DoCameraFOV>d__33>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.IntroGuide.<InitRunTimeScene>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.MenuScene.<HandleRedDot>d__31>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.MenuScene.<StandAloneMode>d__20>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.RunTimeScene.<InitRunTimeScene>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UICommonFunButton.<Set3405BankWeb>d__8>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UICommon_EquipTips.<RefreshLevelUp>d__32>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UICommon_Prompt.<AlphaUpdate>d__12>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Achieve_List.<BottomInit>d__21>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Achieve_List.<CreateTask>d__18>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Activity_Challenge.<CreateTasks>d__65>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Activity_Challenge.<DownSelectSet>d__64>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Activity_EnergyShop.<UpdateConatainerItemTopAsync>d__47>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Activity_Monopoly.<SetPlayerPos>d__95>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Activity_NewSign.<CreateOneDaySign>d__41>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_AnimTools.<Refresh>d__23>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Bank.<Anim>d__36>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_BattleInfo.<OnClickBindings>d__66>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_BattleShop.<CreateBindingItem>d__94>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_BattleShop.<CreateSkillsItem>d__79>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_BattleShop.<Guide>d__66>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_BattleShop.<OnClickBindings>d__70>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_BattleTecnology.<Guide>d__32>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_BattleTecnology.<SetTechBtnItem>d__41>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_BattleTecnology.<SetTechItemUI>d__44>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_BuyEnergy.<Anim>d__40>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Challege.<CreateMainTreadAreaInfo>d__80>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Challege.<SetFromCurrentAreaID>d__75>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Collection_Unlock.<PlayAnim>d__26>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Compound.<InitPanel>d__39>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Compound.<OnSelected>d__34>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Compound.<SpawnItems>d__50>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Compound.<SpawnSelectedCompound>d__47>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_CompoundSuc.<Init>d__7>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_EquipDownGrade.<InitPanel>d__22>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_EquipDownGrade.<RefreshLevelUp>d__23>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_EquipTips.<Guide>d__39>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_EquipTips.<RefreshLevelUp>d__43>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Equipment.<Anim>d__45>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Equipment.<InitPanel>d__59>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Equipment.<InitTab2WidegetInfo>d__65>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Equipment.<OnClickBag>d__72>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Equipment.<OnClickEquip>d__71>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_First_Charge.<CreatTime>d__18>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_GuideSkillChoose.<ChoseOnSkillGroup>d__21>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_GuideTips.<Update>d__13>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_JiyuGame.<Guide>d__39>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_JiyuGame.<OnBtnClickEvent>d__72>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_JiyuGame.<ShopModuleDelayRefresh>d__52>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Login.<GetLocationInfoNew>d__10>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Login.<Init>d__7>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Main.<InitNode>d__89>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Main.<OnBuyEnergyBtnClick>d__97>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Main.<UpdateTreasure>d__108>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_MonopolyTaskShop.<InitNode>d__43>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_MonsterCollection.<CreateBottomBtnList>d__30>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_MonsterCollection.<PlayAnim>d__36>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Notice.<CreateNoticeItem>d__36>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Notice.<SetSpriteByLocalPath>d__35>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Pass.<CreateItem1>d__70>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Pass.<CreateItem>d__80>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Rebirth.<SetTxtTime>d__20>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_RunTimeHUD.<SetEnv>d__85>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_SelectBoxNomal.<InitNode>d__28>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Share.<CreateItems>d__33>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Shop.<Module1201_Help_CreateItem>d__114>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Shop.<Module1302_Help_SetGift>d__124>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Shop.<SetModule1201RedPointState>d__87>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Shop.<SetModule1302RedPointStateHelp>d__91>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Sweep.<ChangeMagnification>d__67>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Talent.<Anim>d__50>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Talent_Prop.<InitNode>d__12>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Task_DailyAndWeekly.<BottomInit>d__54>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Task_DailyAndWeekly.<CreateTask>d__47>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UIPanel_Task_DailyAndWeekly.<TopScoreBoxSet>d__58>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UISubPanel_EnergyShopItem.<OnBtnBuyOnClick>d__23>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UISubPanel_EnergyShopItem.<OnSecBtnClick>d__25>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UISubPanel_EnergyShopItem.<SetReward>d__21>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UISubPanel_Guid.<Refresh>d__46>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UISubPanel_IconBtnItem.<Set3405BankWeb>d__9>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UISubPanel_Pass_Token.<CreateItem>d__24>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UISubPanel_Shop_1102_SBox.<InitEffect>d__49>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UISubPanel_Shop_1301_ChapterGift.<CreateItem>d__33>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UISubPanel_Shop_1301_ChapterGift.<ImgSet>d__29>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UISubPanel_Shop_Gift_Item.<InitEffect>d__17>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<XFramework.UISubPanel_Shop_Pre.<CreateReward>d__26>
	// Cysharp.Threading.Tasks.CompilerServices.IStateMachineRunnerPromise<Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.CompilerServices.IStateMachineRunnerPromise<Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.CompilerServices.IStateMachineRunnerPromise<byte>
	// Cysharp.Threading.Tasks.CompilerServices.IStateMachineRunnerPromise<float>
	// Cysharp.Threading.Tasks.CompilerServices.IStateMachineRunnerPromise<int>
	// Cysharp.Threading.Tasks.CompilerServices.IStateMachineRunnerPromise<object>
	// Cysharp.Threading.Tasks.ITaskPoolNode<object>
	// Cysharp.Threading.Tasks.IUniTaskSource<Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,float>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,Unity.Entities.Entity>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,byte>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,float>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,int>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,object>>
	// Cysharp.Threading.Tasks.IUniTaskSource<Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.IUniTaskSource<byte>
	// Cysharp.Threading.Tasks.IUniTaskSource<float>
	// Cysharp.Threading.Tasks.IUniTaskSource<int>
	// Cysharp.Threading.Tasks.IUniTaskSource<object>
	// Cysharp.Threading.Tasks.Internal.StatePool<Cysharp.Threading.Tasks.UniTask.Awaiter<Cysharp.Threading.Tasks.AsyncUnit>>
	// Cysharp.Threading.Tasks.Internal.StatePool<Cysharp.Threading.Tasks.UniTask.Awaiter<int>>
	// Cysharp.Threading.Tasks.Internal.StatePool<Cysharp.Threading.Tasks.UniTask.Awaiter<object>>
	// Cysharp.Threading.Tasks.Internal.StateTuple<Cysharp.Threading.Tasks.UniTask.Awaiter<Cysharp.Threading.Tasks.AsyncUnit>>
	// Cysharp.Threading.Tasks.Internal.StateTuple<Cysharp.Threading.Tasks.UniTask.Awaiter<int>>
	// Cysharp.Threading.Tasks.Internal.StateTuple<Cysharp.Threading.Tasks.UniTask.Awaiter<object>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,float>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,Unity.Entities.Entity>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,byte>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,float>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,int>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,object>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<byte>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<float>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<int>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<object>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,float>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,Unity.Entities.Entity>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,byte>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,float>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,int>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,object>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<byte>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<float>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<int>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<object>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,float>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,Unity.Entities.Entity>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,byte>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,float>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,int>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,object>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<byte>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<float>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<int>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<object>
	// Cysharp.Threading.Tasks.UniTask<Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,float>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,Unity.Entities.Entity>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,byte>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,float>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,int>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,object>>
	// Cysharp.Threading.Tasks.UniTask<Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.UniTask<byte>
	// Cysharp.Threading.Tasks.UniTask<float>
	// Cysharp.Threading.Tasks.UniTask<int>
	// Cysharp.Threading.Tasks.UniTask<object>
	// Cysharp.Threading.Tasks.UniTaskCompletionSource<byte>
	// Cysharp.Threading.Tasks.UniTaskCompletionSource<object>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<Unity.Entities.Entity>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<byte>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<float>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<int>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<object>
	// Cysharp.Threading.Tasks.UniTaskExtensions.<>c__0<object>
	// Cysharp.Threading.Tasks.UniTaskExtensions.<>c__19<Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.UniTaskExtensions.<>c__19<int>
	// Cysharp.Threading.Tasks.UniTaskExtensions.<>c__19<object>
	// DG.Tweening.Core.DOGetter<UnityEngine.Vector2>
	// DG.Tweening.Core.DOGetter<UnityEngine.Vector3>
	// DG.Tweening.Core.DOGetter<float>
	// DG.Tweening.Core.DOSetter<UnityEngine.Vector2>
	// DG.Tweening.Core.DOSetter<UnityEngine.Vector3>
	// DG.Tweening.Core.DOSetter<float>
	// Google.Protobuf.Collections.MapField.<>c<int,int>
	// Google.Protobuf.Collections.MapField.<>c<int,object>
	// Google.Protobuf.Collections.MapField.<>c__DisplayClass7_0<int,int>
	// Google.Protobuf.Collections.MapField.<>c__DisplayClass7_0<int,object>
	// Google.Protobuf.Collections.MapField.Codec<int,int>
	// Google.Protobuf.Collections.MapField.Codec<int,object>
	// Google.Protobuf.Collections.MapField.DictionaryEnumerator<int,int>
	// Google.Protobuf.Collections.MapField.DictionaryEnumerator<int,object>
	// Google.Protobuf.Collections.MapField.MapView<int,int,int>
	// Google.Protobuf.Collections.MapField.MapView<int,object,int>
	// Google.Protobuf.Collections.MapField.MapView<int,object,object>
	// Google.Protobuf.Collections.MapField<int,int>
	// Google.Protobuf.Collections.MapField<int,object>
	// Google.Protobuf.Collections.RepeatedField.<GetEnumerator>d__28<byte>
	// Google.Protobuf.Collections.RepeatedField.<GetEnumerator>d__28<int>
	// Google.Protobuf.Collections.RepeatedField.<GetEnumerator>d__28<long>
	// Google.Protobuf.Collections.RepeatedField.<GetEnumerator>d__28<object>
	// Google.Protobuf.Collections.RepeatedField<byte>
	// Google.Protobuf.Collections.RepeatedField<int>
	// Google.Protobuf.Collections.RepeatedField<long>
	// Google.Protobuf.Collections.RepeatedField<object>
	// Google.Protobuf.FieldCodec.<>c<byte>
	// Google.Protobuf.FieldCodec.<>c<int>
	// Google.Protobuf.FieldCodec.<>c<long>
	// Google.Protobuf.FieldCodec.<>c<object>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass38_0<byte>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass38_0<int>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass38_0<long>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass38_0<object>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass39_0<byte>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass39_0<int>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass39_0<long>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass39_0<object>
	// Google.Protobuf.FieldCodec.InputMerger<byte>
	// Google.Protobuf.FieldCodec.InputMerger<int>
	// Google.Protobuf.FieldCodec.InputMerger<long>
	// Google.Protobuf.FieldCodec.InputMerger<object>
	// Google.Protobuf.FieldCodec.ValuesMerger<byte>
	// Google.Protobuf.FieldCodec.ValuesMerger<int>
	// Google.Protobuf.FieldCodec.ValuesMerger<long>
	// Google.Protobuf.FieldCodec.ValuesMerger<object>
	// Google.Protobuf.FieldCodec<byte>
	// Google.Protobuf.FieldCodec<int>
	// Google.Protobuf.FieldCodec<long>
	// Google.Protobuf.FieldCodec<object>
	// Google.Protobuf.IDeepCloneable<byte>
	// Google.Protobuf.IDeepCloneable<int>
	// Google.Protobuf.IDeepCloneable<long>
	// Google.Protobuf.IDeepCloneable<object>
	// Google.Protobuf.IMessage<object>
	// Google.Protobuf.MessageParser.<>c__DisplayClass2_0<object>
	// Google.Protobuf.MessageParser<object>
	// Google.Protobuf.ValueReader<byte>
	// Google.Protobuf.ValueReader<int>
	// Google.Protobuf.ValueReader<long>
	// Google.Protobuf.ValueReader<object>
	// Google.Protobuf.ValueWriter<byte>
	// Google.Protobuf.ValueWriter<int>
	// Google.Protobuf.ValueWriter<long>
	// Google.Protobuf.ValueWriter<object>
	// ProtoBuf.Internal.IValueChecker<object>
	// ProtoBuf.Serializers.IFactory<object>
	// ProtoBuf.Serializers.IRepeatedSerializer<object>
	// ProtoBuf.Serializers.ISerializer<object>
	// Spine.ExposedList.Enumerator<object>
	// Spine.ExposedList<object>
	// System.Action<Cysharp.Threading.Tasks.UniTask>
	// System.Action<HotFix_UI.UIAnimationTools.AnimationAlpha>
	// System.Action<HotFix_UI.UIAnimationTools.AnimationScale>
	// System.Action<HotFix_UI.UIAnimationTools.AnimationTran>
	// System.Action<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Action<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Action<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Action<System.ValueTuple<object,object>>
	// System.Action<Unity.Mathematics.int2>
	// System.Action<UnityEngine.Quaternion>
	// System.Action<UnityEngine.Splines.BezierKnot>
	// System.Action<UnityEngine.Vector2>
	// System.Action<UnityEngine.Vector3,UnityEngine.Quaternion>
	// System.Action<UnityEngine.Vector3>
	// System.Action<UnityEngine.Vector4>
	// System.Action<XFramework.ModuleInfoStruct>
	// System.Action<XFramework.TopTabStruct>
	// System.Action<XFramework.UIPanel_AnimTools.AnimToolsStuct>
	// System.Action<XFramework.UIPanel_BattleShop.BattleShopDrop>
	// System.Action<XFramework.UISubPanel_Guid.AnimToolsStuct>
	// System.Action<byte>
	// System.Action<float>
	// System.Action<int,int>
	// System.Action<int>
	// System.Action<long,object>
	// System.Action<long>
	// System.Action<object,object>
	// System.Action<object>
	// System.ByReference<Main.Buff>
	// System.ByReference<Main.BuffOld>
	// System.ByReference<Main.ChaStats>
	// System.ByReference<Main.DamageInfo>
	// System.ByReference<Main.GameEvent>
	// System.ByReference<Main.GameOthersData>
	// System.ByReference<Main.GlobalConfigData>
	// System.ByReference<Main.PlayerData>
	// System.ByReference<Main.PrefabMapData>
	// System.ByReference<Main.Skill>
	// System.ByReference<Main.State>
	// System.ByReference<Main.Trigger>
	// System.ByReference<Unity.Collections.FixedString128Bytes>
	// System.ByReference<Unity.Entities.ComponentType>
	// System.ByReference<Unity.Entities.Entity>
	// System.ByReference<Unity.Entities.EntityQuery>
	// System.ByReference<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// System.ByReference<Unity.Entities.LinkedEntityGroup>
	// System.ByReference<Unity.Transforms.LocalTransform>
	// System.ByReference<UnityEngine.jvalue>
	// System.ByReference<int>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<object>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<object>
	// System.Collections.Concurrent.ConcurrentQueue<object>
	// System.Collections.Generic.ArraySortHelper<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.ArraySortHelper<HotFix_UI.UIAnimationTools.AnimationAlpha>
	// System.Collections.Generic.ArraySortHelper<HotFix_UI.UIAnimationTools.AnimationScale>
	// System.Collections.Generic.ArraySortHelper<HotFix_UI.UIAnimationTools.AnimationTran>
	// System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ArraySortHelper<System.ValueTuple<object,object>>
	// System.Collections.Generic.ArraySortHelper<Unity.Mathematics.int2>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Splines.BezierKnot>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Vector2>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Vector3>
	// System.Collections.Generic.ArraySortHelper<XFramework.ModuleInfoStruct>
	// System.Collections.Generic.ArraySortHelper<XFramework.TopTabStruct>
	// System.Collections.Generic.ArraySortHelper<XFramework.UIPanel_AnimTools.AnimToolsStuct>
	// System.Collections.Generic.ArraySortHelper<XFramework.UIPanel_BattleShop.BattleShopDrop>
	// System.Collections.Generic.ArraySortHelper<XFramework.UISubPanel_Guid.AnimToolsStuct>
	// System.Collections.Generic.ArraySortHelper<byte>
	// System.Collections.Generic.ArraySortHelper<float>
	// System.Collections.Generic.ArraySortHelper<int>
	// System.Collections.Generic.ArraySortHelper<long>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<Cysharp.Threading.Tasks.AsyncUnit>
	// System.Collections.Generic.Comparer<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.Comparer<HotFix_UI.UIAnimationTools.AnimationAlpha>
	// System.Collections.Generic.Comparer<HotFix_UI.UIAnimationTools.AnimationScale>
	// System.Collections.Generic.Comparer<HotFix_UI.UIAnimationTools.AnimationTran>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,float>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,byte>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,float>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,int>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,object>>
	// System.Collections.Generic.Comparer<System.ValueTuple<object,object>>
	// System.Collections.Generic.Comparer<Unity.Entities.Entity>
	// System.Collections.Generic.Comparer<Unity.Mathematics.int2>
	// System.Collections.Generic.Comparer<UnityEngine.Splines.BezierKnot>
	// System.Collections.Generic.Comparer<UnityEngine.Vector2>
	// System.Collections.Generic.Comparer<UnityEngine.Vector3>
	// System.Collections.Generic.Comparer<XFramework.ModuleInfoStruct>
	// System.Collections.Generic.Comparer<XFramework.TopTabStruct>
	// System.Collections.Generic.Comparer<XFramework.UIPanel_AnimTools.AnimToolsStuct>
	// System.Collections.Generic.Comparer<XFramework.UIPanel_BattleShop.BattleShopDrop>
	// System.Collections.Generic.Comparer<XFramework.UISubPanel_Guid.AnimToolsStuct>
	// System.Collections.Generic.Comparer<byte>
	// System.Collections.Generic.Comparer<float>
	// System.Collections.Generic.Comparer<int>
	// System.Collections.Generic.Comparer<long>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<UnityEngine.Vector2,long>
	// System.Collections.Generic.Dictionary.Enumerator<UnityEngine.Vector3,int>
	// System.Collections.Generic.Dictionary.Enumerator<UnityEngine.Vector3,long>
	// System.Collections.Generic.Dictionary.Enumerator<int,UnityEngine.Vector2>
	// System.Collections.Generic.Dictionary.Enumerator<int,byte>
	// System.Collections.Generic.Dictionary.Enumerator<int,float>
	// System.Collections.Generic.Dictionary.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.Enumerator<int,long>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<long,int>
	// System.Collections.Generic.Dictionary.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.Dictionary.Enumerator<object,XFramework.GameObjectPool.MyRect>
	// System.Collections.Generic.Dictionary.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<UnityEngine.Vector2,long>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<UnityEngine.Vector3,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<UnityEngine.Vector3,long>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,UnityEngine.Vector2>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,byte>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,float>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,long>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,XFramework.GameObjectPool.MyRect>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<UnityEngine.Vector2,long>
	// System.Collections.Generic.Dictionary.KeyCollection<UnityEngine.Vector3,int>
	// System.Collections.Generic.Dictionary.KeyCollection<UnityEngine.Vector3,long>
	// System.Collections.Generic.Dictionary.KeyCollection<int,UnityEngine.Vector2>
	// System.Collections.Generic.Dictionary.KeyCollection<int,byte>
	// System.Collections.Generic.Dictionary.KeyCollection<int,float>
	// System.Collections.Generic.Dictionary.KeyCollection<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection<int,long>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<long,int>
	// System.Collections.Generic.Dictionary.KeyCollection<long,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.Dictionary.KeyCollection<object,XFramework.GameObjectPool.MyRect>
	// System.Collections.Generic.Dictionary.KeyCollection<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<UnityEngine.Vector2,long>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<UnityEngine.Vector3,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<UnityEngine.Vector3,long>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,UnityEngine.Vector2>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,byte>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,float>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,long>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,XFramework.GameObjectPool.MyRect>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<UnityEngine.Vector2,long>
	// System.Collections.Generic.Dictionary.ValueCollection<UnityEngine.Vector3,int>
	// System.Collections.Generic.Dictionary.ValueCollection<UnityEngine.Vector3,long>
	// System.Collections.Generic.Dictionary.ValueCollection<int,UnityEngine.Vector2>
	// System.Collections.Generic.Dictionary.ValueCollection<int,byte>
	// System.Collections.Generic.Dictionary.ValueCollection<int,float>
	// System.Collections.Generic.Dictionary.ValueCollection<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection<int,long>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<long,int>
	// System.Collections.Generic.Dictionary.ValueCollection<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.Dictionary.ValueCollection<object,XFramework.GameObjectPool.MyRect>
	// System.Collections.Generic.Dictionary.ValueCollection<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary<UnityEngine.Vector2,long>
	// System.Collections.Generic.Dictionary<UnityEngine.Vector3,int>
	// System.Collections.Generic.Dictionary<UnityEngine.Vector3,long>
	// System.Collections.Generic.Dictionary<int,UnityEngine.Vector2>
	// System.Collections.Generic.Dictionary<int,byte>
	// System.Collections.Generic.Dictionary<int,float>
	// System.Collections.Generic.Dictionary<int,int>
	// System.Collections.Generic.Dictionary<int,long>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<long,int>
	// System.Collections.Generic.Dictionary<long,object>
	// System.Collections.Generic.Dictionary<object,Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.Dictionary<object,XFramework.GameObjectPool.MyRect>
	// System.Collections.Generic.Dictionary<object,int>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.EqualityComparer<Cysharp.Threading.Tasks.AsyncUnit>
	// System.Collections.Generic.EqualityComparer<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.EqualityComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,float>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,byte>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,float>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,int>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,object>>
	// System.Collections.Generic.EqualityComparer<Unity.Entities.Entity>
	// System.Collections.Generic.EqualityComparer<Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.EqualityComparer<UnityEngine.Vector2>
	// System.Collections.Generic.EqualityComparer<UnityEngine.Vector3>
	// System.Collections.Generic.EqualityComparer<XFramework.GameObjectPool.MyRect>
	// System.Collections.Generic.EqualityComparer<byte>
	// System.Collections.Generic.EqualityComparer<float>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<long>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.HashSet.Enumerator<int>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet<int>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.HashSetEqualityComparer<int>
	// System.Collections.Generic.HashSetEqualityComparer<object>
	// System.Collections.Generic.ICollection<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.ICollection<HotFix_UI.UIAnimationTools.AnimationAlpha>
	// System.Collections.Generic.ICollection<HotFix_UI.UIAnimationTools.AnimationScale>
	// System.Collections.Generic.ICollection<HotFix_UI.UIAnimationTools.AnimationTran>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<UnityEngine.Vector2,long>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<UnityEngine.Vector3,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<UnityEngine.Vector3,long>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,UnityEngine.Vector2>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,float>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,long>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,Unity.Entities.SystemTypeIndex>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,XFramework.GameObjectPool.MyRect>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<System.ValueTuple<object,object>>
	// System.Collections.Generic.ICollection<Unity.Mathematics.int2>
	// System.Collections.Generic.ICollection<UnityEngine.Splines.BezierKnot>
	// System.Collections.Generic.ICollection<UnityEngine.Vector2>
	// System.Collections.Generic.ICollection<UnityEngine.Vector3>
	// System.Collections.Generic.ICollection<XFramework.ModuleInfoStruct>
	// System.Collections.Generic.ICollection<XFramework.TopTabStruct>
	// System.Collections.Generic.ICollection<XFramework.UIPanel_AnimTools.AnimToolsStuct>
	// System.Collections.Generic.ICollection<XFramework.UIPanel_BattleShop.BattleShopDrop>
	// System.Collections.Generic.ICollection<XFramework.UISubPanel_Guid.AnimToolsStuct>
	// System.Collections.Generic.ICollection<byte>
	// System.Collections.Generic.ICollection<float>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<long>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.ICollection<ushort>
	// System.Collections.Generic.IComparer<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.IComparer<HotFix_UI.UIAnimationTools.AnimationAlpha>
	// System.Collections.Generic.IComparer<HotFix_UI.UIAnimationTools.AnimationScale>
	// System.Collections.Generic.IComparer<HotFix_UI.UIAnimationTools.AnimationTran>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IComparer<System.ValueTuple<object,object>>
	// System.Collections.Generic.IComparer<Unity.Mathematics.int2>
	// System.Collections.Generic.IComparer<UnityEngine.Splines.BezierKnot>
	// System.Collections.Generic.IComparer<UnityEngine.Vector2>
	// System.Collections.Generic.IComparer<UnityEngine.Vector3>
	// System.Collections.Generic.IComparer<XFramework.ModuleInfoStruct>
	// System.Collections.Generic.IComparer<XFramework.TopTabStruct>
	// System.Collections.Generic.IComparer<XFramework.UIPanel_AnimTools.AnimToolsStuct>
	// System.Collections.Generic.IComparer<XFramework.UIPanel_BattleShop.BattleShopDrop>
	// System.Collections.Generic.IComparer<XFramework.UISubPanel_Guid.AnimToolsStuct>
	// System.Collections.Generic.IComparer<byte>
	// System.Collections.Generic.IComparer<float>
	// System.Collections.Generic.IComparer<int>
	// System.Collections.Generic.IComparer<long>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IDictionary<int,int>
	// System.Collections.Generic.IDictionary<int,object>
	// System.Collections.Generic.IDictionary<long,object>
	// System.Collections.Generic.IDictionary<object,double>
	// System.Collections.Generic.IDictionary<object,float>
	// System.Collections.Generic.IDictionary<object,int>
	// System.Collections.Generic.IDictionary<object,long>
	// System.Collections.Generic.IDictionary<object,object>
	// System.Collections.Generic.IEnumerable<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.IEnumerable<HotFix_UI.UIAnimationTools.AnimationAlpha>
	// System.Collections.Generic.IEnumerable<HotFix_UI.UIAnimationTools.AnimationScale>
	// System.Collections.Generic.IEnumerable<HotFix_UI.UIAnimationTools.AnimationTran>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<UnityEngine.Vector2,long>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<UnityEngine.Vector3,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<UnityEngine.Vector3,long>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,UnityEngine.Vector2>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,float>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,long>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,Unity.Entities.SystemTypeIndex>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,XFramework.GameObjectPool.MyRect>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<System.ValueTuple<object,object>>
	// System.Collections.Generic.IEnumerable<Unity.Mathematics.int2>
	// System.Collections.Generic.IEnumerable<UnityEngine.Splines.BezierKnot>
	// System.Collections.Generic.IEnumerable<UnityEngine.Vector2>
	// System.Collections.Generic.IEnumerable<UnityEngine.Vector3>
	// System.Collections.Generic.IEnumerable<XFramework.ModuleInfoStruct>
	// System.Collections.Generic.IEnumerable<XFramework.TopTabStruct>
	// System.Collections.Generic.IEnumerable<XFramework.UIPanel_AnimTools.AnimToolsStuct>
	// System.Collections.Generic.IEnumerable<XFramework.UIPanel_BattleShop.BattleShopDrop>
	// System.Collections.Generic.IEnumerable<XFramework.UISubPanel_Guid.AnimToolsStuct>
	// System.Collections.Generic.IEnumerable<byte>
	// System.Collections.Generic.IEnumerable<float>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<long>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerable<ushort>
	// System.Collections.Generic.IEnumerator<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.IEnumerator<HotFix_UI.UIAnimationTools.AnimationAlpha>
	// System.Collections.Generic.IEnumerator<HotFix_UI.UIAnimationTools.AnimationScale>
	// System.Collections.Generic.IEnumerator<HotFix_UI.UIAnimationTools.AnimationTran>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<UnityEngine.Vector2,long>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<UnityEngine.Vector3,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<UnityEngine.Vector3,long>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,UnityEngine.Vector2>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,byte>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,float>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,long>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,Unity.Entities.SystemTypeIndex>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,XFramework.GameObjectPool.MyRect>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<System.ValueTuple<object,object>>
	// System.Collections.Generic.IEnumerator<Unity.Entities.QueryEnumerableWithEntity<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRO<Main.GameOthersData>>>
	// System.Collections.Generic.IEnumerator<Unity.Mathematics.int2>
	// System.Collections.Generic.IEnumerator<UnityEngine.Splines.BezierKnot>
	// System.Collections.Generic.IEnumerator<UnityEngine.Vector2>
	// System.Collections.Generic.IEnumerator<UnityEngine.Vector3>
	// System.Collections.Generic.IEnumerator<XFramework.ModuleInfoStruct>
	// System.Collections.Generic.IEnumerator<XFramework.TopTabStruct>
	// System.Collections.Generic.IEnumerator<XFramework.UIPanel_AnimTools.AnimToolsStuct>
	// System.Collections.Generic.IEnumerator<XFramework.UIPanel_BattleShop.BattleShopDrop>
	// System.Collections.Generic.IEnumerator<XFramework.UISubPanel_Guid.AnimToolsStuct>
	// System.Collections.Generic.IEnumerator<byte>
	// System.Collections.Generic.IEnumerator<float>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<long>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEqualityComparer<UnityEngine.Vector2>
	// System.Collections.Generic.IEqualityComparer<UnityEngine.Vector3>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<long>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IList<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.IList<HotFix_UI.UIAnimationTools.AnimationAlpha>
	// System.Collections.Generic.IList<HotFix_UI.UIAnimationTools.AnimationScale>
	// System.Collections.Generic.IList<HotFix_UI.UIAnimationTools.AnimationTran>
	// System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<int,long>>
	// System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IList<System.ValueTuple<object,object>>
	// System.Collections.Generic.IList<Unity.Mathematics.int2>
	// System.Collections.Generic.IList<UnityEngine.Splines.BezierKnot>
	// System.Collections.Generic.IList<UnityEngine.Vector2>
	// System.Collections.Generic.IList<UnityEngine.Vector3>
	// System.Collections.Generic.IList<XFramework.ModuleInfoStruct>
	// System.Collections.Generic.IList<XFramework.TopTabStruct>
	// System.Collections.Generic.IList<XFramework.UIPanel_AnimTools.AnimToolsStuct>
	// System.Collections.Generic.IList<XFramework.UIPanel_BattleShop.BattleShopDrop>
	// System.Collections.Generic.IList<XFramework.UISubPanel_Guid.AnimToolsStuct>
	// System.Collections.Generic.IList<byte>
	// System.Collections.Generic.IList<float>
	// System.Collections.Generic.IList<int>
	// System.Collections.Generic.IList<long>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.KeyValuePair<UnityEngine.Vector2,long>
	// System.Collections.Generic.KeyValuePair<UnityEngine.Vector3,int>
	// System.Collections.Generic.KeyValuePair<UnityEngine.Vector3,long>
	// System.Collections.Generic.KeyValuePair<int,UnityEngine.Vector2>
	// System.Collections.Generic.KeyValuePair<int,byte>
	// System.Collections.Generic.KeyValuePair<int,float>
	// System.Collections.Generic.KeyValuePair<int,int>
	// System.Collections.Generic.KeyValuePair<int,long>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<long,int>
	// System.Collections.Generic.KeyValuePair<long,object>
	// System.Collections.Generic.KeyValuePair<object,Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.KeyValuePair<object,XFramework.GameObjectPool.MyRect>
	// System.Collections.Generic.KeyValuePair<object,int>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.LinkedList.Enumerator<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.LinkedList.Enumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.LinkedList<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.LinkedList<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.LinkedListNode<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.LinkedListNode<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.List.Enumerator<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.List.Enumerator<HotFix_UI.UIAnimationTools.AnimationAlpha>
	// System.Collections.Generic.List.Enumerator<HotFix_UI.UIAnimationTools.AnimationScale>
	// System.Collections.Generic.List.Enumerator<HotFix_UI.UIAnimationTools.AnimationTran>
	// System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.List.Enumerator<System.ValueTuple<object,object>>
	// System.Collections.Generic.List.Enumerator<Unity.Mathematics.int2>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Splines.BezierKnot>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Vector2>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Vector3>
	// System.Collections.Generic.List.Enumerator<XFramework.ModuleInfoStruct>
	// System.Collections.Generic.List.Enumerator<XFramework.TopTabStruct>
	// System.Collections.Generic.List.Enumerator<XFramework.UIPanel_AnimTools.AnimToolsStuct>
	// System.Collections.Generic.List.Enumerator<XFramework.UIPanel_BattleShop.BattleShopDrop>
	// System.Collections.Generic.List.Enumerator<XFramework.UISubPanel_Guid.AnimToolsStuct>
	// System.Collections.Generic.List.Enumerator<byte>
	// System.Collections.Generic.List.Enumerator<float>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<long>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.List<HotFix_UI.UIAnimationTools.AnimationAlpha>
	// System.Collections.Generic.List<HotFix_UI.UIAnimationTools.AnimationScale>
	// System.Collections.Generic.List<HotFix_UI.UIAnimationTools.AnimationTran>
	// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.List<System.ValueTuple<object,object>>
	// System.Collections.Generic.List<Unity.Mathematics.int2>
	// System.Collections.Generic.List<UnityEngine.Splines.BezierKnot>
	// System.Collections.Generic.List<UnityEngine.Vector2>
	// System.Collections.Generic.List<UnityEngine.Vector3>
	// System.Collections.Generic.List<XFramework.ModuleInfoStruct>
	// System.Collections.Generic.List<XFramework.TopTabStruct>
	// System.Collections.Generic.List<XFramework.UIPanel_AnimTools.AnimToolsStuct>
	// System.Collections.Generic.List<XFramework.UIPanel_BattleShop.BattleShopDrop>
	// System.Collections.Generic.List<XFramework.UISubPanel_Guid.AnimToolsStuct>
	// System.Collections.Generic.List<byte>
	// System.Collections.Generic.List<float>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<long>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<Cysharp.Threading.Tasks.AsyncUnit>
	// System.Collections.Generic.ObjectComparer<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.ObjectComparer<HotFix_UI.UIAnimationTools.AnimationAlpha>
	// System.Collections.Generic.ObjectComparer<HotFix_UI.UIAnimationTools.AnimationScale>
	// System.Collections.Generic.ObjectComparer<HotFix_UI.UIAnimationTools.AnimationTran>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,float>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,byte>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,float>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,int>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,object>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<object,object>>
	// System.Collections.Generic.ObjectComparer<Unity.Entities.Entity>
	// System.Collections.Generic.ObjectComparer<Unity.Mathematics.int2>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Splines.BezierKnot>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Vector2>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Vector3>
	// System.Collections.Generic.ObjectComparer<XFramework.ModuleInfoStruct>
	// System.Collections.Generic.ObjectComparer<XFramework.TopTabStruct>
	// System.Collections.Generic.ObjectComparer<XFramework.UIPanel_AnimTools.AnimToolsStuct>
	// System.Collections.Generic.ObjectComparer<XFramework.UIPanel_BattleShop.BattleShopDrop>
	// System.Collections.Generic.ObjectComparer<XFramework.UISubPanel_Guid.AnimToolsStuct>
	// System.Collections.Generic.ObjectComparer<byte>
	// System.Collections.Generic.ObjectComparer<float>
	// System.Collections.Generic.ObjectComparer<int>
	// System.Collections.Generic.ObjectComparer<long>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<Cysharp.Threading.Tasks.AsyncUnit>
	// System.Collections.Generic.ObjectEqualityComparer<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.ObjectEqualityComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,float>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,byte>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,float>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,int>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,object>>
	// System.Collections.Generic.ObjectEqualityComparer<Unity.Entities.Entity>
	// System.Collections.Generic.ObjectEqualityComparer<Unity.Entities.SystemTypeIndex>
	// System.Collections.Generic.ObjectEqualityComparer<UnityEngine.Vector2>
	// System.Collections.Generic.ObjectEqualityComparer<UnityEngine.Vector3>
	// System.Collections.Generic.ObjectEqualityComparer<XFramework.GameObjectPool.MyRect>
	// System.Collections.Generic.ObjectEqualityComparer<byte>
	// System.Collections.Generic.ObjectEqualityComparer<float>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<long>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.Queue.Enumerator<long>
	// System.Collections.Generic.Queue.Enumerator<object>
	// System.Collections.Generic.Queue<long>
	// System.Collections.Generic.Queue<object>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_0<long,object>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_0<object,object>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_1<long,object>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_1<object,object>
	// System.Collections.Generic.SortedDictionary.Enumerator<long,object>
	// System.Collections.Generic.SortedDictionary.Enumerator<object,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass5_0<long,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass5_0<object,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass6_0<long,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass6_0<object,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.Enumerator<long,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection<long,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection<object,object>
	// System.Collections.Generic.SortedDictionary.KeyValuePairComparer<long,object>
	// System.Collections.Generic.SortedDictionary.KeyValuePairComparer<object,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass5_0<long,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass5_0<object,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass6_0<long,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass6_0<object,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.Enumerator<long,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection<long,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection<object,object>
	// System.Collections.Generic.SortedDictionary<long,object>
	// System.Collections.Generic.SortedDictionary<object,object>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass52_0<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass52_0<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass53_0<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass53_0<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.SortedSet.Enumerator<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.Enumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.SortedSet.Node<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.Node<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.SortedSet<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.Stack.Enumerator<object>
	// System.Collections.Generic.Stack<object>
	// System.Collections.Generic.TreeSet<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.TreeSet<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.TreeWalkPredicate<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.TreeWalkPredicate<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.ObjectModel.ReadOnlyCollection<HotFix_UI.UIAnimationTools.AnimationAlpha>
	// System.Collections.ObjectModel.ReadOnlyCollection<HotFix_UI.UIAnimationTools.AnimationScale>
	// System.Collections.ObjectModel.ReadOnlyCollection<HotFix_UI.UIAnimationTools.AnimationTran>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.ValueTuple<object,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<Unity.Mathematics.int2>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Splines.BezierKnot>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector2>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector3>
	// System.Collections.ObjectModel.ReadOnlyCollection<XFramework.ModuleInfoStruct>
	// System.Collections.ObjectModel.ReadOnlyCollection<XFramework.TopTabStruct>
	// System.Collections.ObjectModel.ReadOnlyCollection<XFramework.UIPanel_AnimTools.AnimToolsStuct>
	// System.Collections.ObjectModel.ReadOnlyCollection<XFramework.UIPanel_BattleShop.BattleShopDrop>
	// System.Collections.ObjectModel.ReadOnlyCollection<XFramework.UISubPanel_Guid.AnimToolsStuct>
	// System.Collections.ObjectModel.ReadOnlyCollection<byte>
	// System.Collections.ObjectModel.ReadOnlyCollection<float>
	// System.Collections.ObjectModel.ReadOnlyCollection<int>
	// System.Collections.ObjectModel.ReadOnlyCollection<long>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<Cysharp.Threading.Tasks.UniTask>
	// System.Comparison<HotFix_UI.UIAnimationTools.AnimationAlpha>
	// System.Comparison<HotFix_UI.UIAnimationTools.AnimationScale>
	// System.Comparison<HotFix_UI.UIAnimationTools.AnimationTran>
	// System.Comparison<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Comparison<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Comparison<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Comparison<System.ValueTuple<object,object>>
	// System.Comparison<Unity.Mathematics.int2>
	// System.Comparison<UnityEngine.Splines.BezierKnot>
	// System.Comparison<UnityEngine.Vector2>
	// System.Comparison<UnityEngine.Vector3>
	// System.Comparison<XFramework.ModuleInfoStruct>
	// System.Comparison<XFramework.TopTabStruct>
	// System.Comparison<XFramework.UIPanel_AnimTools.AnimToolsStuct>
	// System.Comparison<XFramework.UIPanel_BattleShop.BattleShopDrop>
	// System.Comparison<XFramework.UISubPanel_Guid.AnimToolsStuct>
	// System.Comparison<byte>
	// System.Comparison<float>
	// System.Comparison<int>
	// System.Comparison<long>
	// System.Comparison<object>
	// System.EventHandler<object>
	// System.Func<Cysharp.Threading.Tasks.UniTaskVoid>
	// System.Func<System.Collections.Generic.KeyValuePair<int,float>,float>
	// System.Func<System.Collections.Generic.KeyValuePair<int,float>,int>
	// System.Func<System.Collections.Generic.KeyValuePair<int,int>,System.Collections.DictionaryEntry>
	// System.Func<System.Collections.Generic.KeyValuePair<int,int>,byte>
	// System.Func<System.Collections.Generic.KeyValuePair<int,int>,int>
	// System.Func<System.Collections.Generic.KeyValuePair<int,object>,System.Collections.DictionaryEntry>
	// System.Func<System.Collections.Generic.KeyValuePair<int,object>,byte>
	// System.Func<System.Collections.Generic.KeyValuePair<int,object>,int>
	// System.Func<System.Collections.Generic.KeyValuePair<int,object>,object>
	// System.Func<System.Collections.Generic.KeyValuePair<long,object>,byte>
	// System.Func<System.Collections.Generic.KeyValuePair<object,int>,int>
	// System.Func<System.Collections.Generic.KeyValuePair<object,int>,object>
	// System.Func<UnityEngine.Vector3,byte>
	// System.Func<UnityEngine.Vector3,float>
	// System.Func<UnityEngine.Vector3>
	// System.Func<byte,int>
	// System.Func<byte>
	// System.Func<int,byte>
	// System.Func<int,int,object>
	// System.Func<int,int>
	// System.Func<int,object>
	// System.Func<int>
	// System.Func<long,int>
	// System.Func<object,Cysharp.Threading.Tasks.UniTask<object>>
	// System.Func<object,byte>
	// System.Func<object,float>
	// System.Func<object,int,int,int,object>
	// System.Func<object,int,object>
	// System.Func<object,int>
	// System.Func<object,object,object>
	// System.Func<object,object>
	// System.Func<object>
	// System.IComparable<object>
	// System.IEquatable<Unity.Collections.FixedString128Bytes>
	// System.IEquatable<int>
	// System.IEquatable<object>
	// System.Linq.Buffer<System.Collections.Generic.KeyValuePair<int,float>>
	// System.Linq.Buffer<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Linq.Buffer<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Linq.Buffer<UnityEngine.Vector3>
	// System.Linq.Buffer<int>
	// System.Linq.Buffer<object>
	// System.Linq.Enumerable.Iterator<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Linq.Enumerable.Iterator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Linq.Enumerable.Iterator<UnityEngine.Vector3>
	// System.Linq.Enumerable.Iterator<int>
	// System.Linq.Enumerable.Iterator<object>
	// System.Linq.Enumerable.WhereArrayIterator<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Linq.Enumerable.WhereArrayIterator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Linq.Enumerable.WhereArrayIterator<UnityEngine.Vector3>
	// System.Linq.Enumerable.WhereArrayIterator<int>
	// System.Linq.Enumerable.WhereArrayIterator<object>
	// System.Linq.Enumerable.WhereEnumerableIterator<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Linq.Enumerable.WhereEnumerableIterator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Linq.Enumerable.WhereEnumerableIterator<UnityEngine.Vector3>
	// System.Linq.Enumerable.WhereEnumerableIterator<int>
	// System.Linq.Enumerable.WhereEnumerableIterator<object>
	// System.Linq.Enumerable.WhereListIterator<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Linq.Enumerable.WhereListIterator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Linq.Enumerable.WhereListIterator<UnityEngine.Vector3>
	// System.Linq.Enumerable.WhereListIterator<int>
	// System.Linq.Enumerable.WhereListIterator<object>
	// System.Linq.Enumerable.WhereSelectArrayIterator<System.Collections.Generic.KeyValuePair<int,int>,int>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,int>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,object>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<System.Collections.Generic.KeyValuePair<int,int>,int>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,int>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,object>
	// System.Linq.Enumerable.WhereSelectListIterator<System.Collections.Generic.KeyValuePair<int,int>,int>
	// System.Linq.Enumerable.WhereSelectListIterator<object,int>
	// System.Linq.Enumerable.WhereSelectListIterator<object,object>
	// System.Linq.EnumerableSorter<System.Collections.Generic.KeyValuePair<int,float>,float>
	// System.Linq.EnumerableSorter<System.Collections.Generic.KeyValuePair<int,float>>
	// System.Linq.EnumerableSorter<System.Collections.Generic.KeyValuePair<int,int>,int>
	// System.Linq.EnumerableSorter<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Linq.EnumerableSorter<System.Collections.Generic.KeyValuePair<object,int>,int>
	// System.Linq.EnumerableSorter<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Linq.EnumerableSorter<UnityEngine.Vector3,float>
	// System.Linq.EnumerableSorter<UnityEngine.Vector3>
	// System.Linq.EnumerableSorter<int,int>
	// System.Linq.EnumerableSorter<int>
	// System.Linq.EnumerableSorter<object,float>
	// System.Linq.EnumerableSorter<object,int>
	// System.Linq.EnumerableSorter<object>
	// System.Linq.GroupedEnumerable<object,int,object>
	// System.Linq.IOrderedEnumerable<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Linq.IdentityFunction.<>c<object>
	// System.Linq.IdentityFunction<object>
	// System.Linq.Lookup.<GetEnumerator>d__12<int,object>
	// System.Linq.Lookup.Grouping.<GetEnumerator>d__7<int,object>
	// System.Linq.Lookup.Grouping<int,object>
	// System.Linq.Lookup<int,object>
	// System.Linq.OrderedEnumerable.<GetEnumerator>d__1<System.Collections.Generic.KeyValuePair<int,float>>
	// System.Linq.OrderedEnumerable.<GetEnumerator>d__1<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Linq.OrderedEnumerable.<GetEnumerator>d__1<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Linq.OrderedEnumerable.<GetEnumerator>d__1<UnityEngine.Vector3>
	// System.Linq.OrderedEnumerable.<GetEnumerator>d__1<int>
	// System.Linq.OrderedEnumerable.<GetEnumerator>d__1<object>
	// System.Linq.OrderedEnumerable<System.Collections.Generic.KeyValuePair<int,float>,float>
	// System.Linq.OrderedEnumerable<System.Collections.Generic.KeyValuePair<int,float>>
	// System.Linq.OrderedEnumerable<System.Collections.Generic.KeyValuePair<int,int>,int>
	// System.Linq.OrderedEnumerable<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Linq.OrderedEnumerable<System.Collections.Generic.KeyValuePair<object,int>,int>
	// System.Linq.OrderedEnumerable<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Linq.OrderedEnumerable<UnityEngine.Vector3,float>
	// System.Linq.OrderedEnumerable<UnityEngine.Vector3>
	// System.Linq.OrderedEnumerable<int,int>
	// System.Linq.OrderedEnumerable<int>
	// System.Linq.OrderedEnumerable<object,float>
	// System.Linq.OrderedEnumerable<object,int>
	// System.Linq.OrderedEnumerable<object>
	// System.Nullable<float>
	// System.Nullable<int>
	// System.Predicate<Cysharp.Threading.Tasks.UniTask>
	// System.Predicate<HotFix_UI.UIAnimationTools.AnimationAlpha>
	// System.Predicate<HotFix_UI.UIAnimationTools.AnimationScale>
	// System.Predicate<HotFix_UI.UIAnimationTools.AnimationTran>
	// System.Predicate<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Predicate<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Predicate<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Predicate<System.ValueTuple<object,object>>
	// System.Predicate<Unity.Mathematics.int2>
	// System.Predicate<UnityEngine.Splines.BezierKnot>
	// System.Predicate<UnityEngine.Vector2>
	// System.Predicate<UnityEngine.Vector3>
	// System.Predicate<XFramework.ModuleInfoStruct>
	// System.Predicate<XFramework.TopTabStruct>
	// System.Predicate<XFramework.UIPanel_AnimTools.AnimToolsStuct>
	// System.Predicate<XFramework.UIPanel_BattleShop.BattleShopDrop>
	// System.Predicate<XFramework.UISubPanel_Guid.AnimToolsStuct>
	// System.Predicate<byte>
	// System.Predicate<float>
	// System.Predicate<int>
	// System.Predicate<long>
	// System.Predicate<object>
	// System.ReadOnlySpan<Main.Buff>
	// System.ReadOnlySpan<Main.BuffOld>
	// System.ReadOnlySpan<Main.ChaStats>
	// System.ReadOnlySpan<Main.DamageInfo>
	// System.ReadOnlySpan<Main.GameEvent>
	// System.ReadOnlySpan<Main.GameOthersData>
	// System.ReadOnlySpan<Main.GlobalConfigData>
	// System.ReadOnlySpan<Main.PlayerData>
	// System.ReadOnlySpan<Main.PrefabMapData>
	// System.ReadOnlySpan<Main.Skill>
	// System.ReadOnlySpan<Main.State>
	// System.ReadOnlySpan<Main.Trigger>
	// System.ReadOnlySpan<Unity.Collections.FixedString128Bytes>
	// System.ReadOnlySpan<Unity.Entities.ComponentType>
	// System.ReadOnlySpan<Unity.Entities.Entity>
	// System.ReadOnlySpan<Unity.Entities.EntityQuery>
	// System.ReadOnlySpan<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// System.ReadOnlySpan<Unity.Entities.LinkedEntityGroup>
	// System.ReadOnlySpan<Unity.Transforms.LocalTransform>
	// System.ReadOnlySpan<UnityEngine.jvalue>
	// System.ReadOnlySpan<int>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<object>
	// System.Runtime.CompilerServices.TaskAwaiter<object>
	// System.Span<Main.Buff>
	// System.Span<Main.BuffOld>
	// System.Span<Main.ChaStats>
	// System.Span<Main.DamageInfo>
	// System.Span<Main.GameEvent>
	// System.Span<Main.GameOthersData>
	// System.Span<Main.GlobalConfigData>
	// System.Span<Main.PlayerData>
	// System.Span<Main.PrefabMapData>
	// System.Span<Main.Skill>
	// System.Span<Main.State>
	// System.Span<Main.Trigger>
	// System.Span<Unity.Collections.FixedString128Bytes>
	// System.Span<Unity.Entities.ComponentType>
	// System.Span<Unity.Entities.Entity>
	// System.Span<Unity.Entities.EntityQuery>
	// System.Span<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// System.Span<Unity.Entities.LinkedEntityGroup>
	// System.Span<Unity.Transforms.LocalTransform>
	// System.Span<UnityEngine.jvalue>
	// System.Span<int>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<object>
	// System.Threading.Tasks.Task<object>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<object>
	// System.Threading.Tasks.TaskFactory<object>
	// System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>
	// System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Cysharp.Threading.Tasks.AsyncUnit>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,float>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,Unity.Entities.Entity>>
	// System.ValueTuple<byte,System.ValueTuple<byte,byte>>
	// System.ValueTuple<byte,System.ValueTuple<byte,float>>
	// System.ValueTuple<byte,System.ValueTuple<byte,int>>
	// System.ValueTuple<byte,System.ValueTuple<byte,object>>
	// System.ValueTuple<byte,Unity.Entities.Entity>
	// System.ValueTuple<byte,byte>
	// System.ValueTuple<byte,float>
	// System.ValueTuple<byte,int>
	// System.ValueTuple<byte,object>
	// System.ValueTuple<object,object>
	// Unity.Burst.SharedStatic<Unity.Collections.Long1024>
	// Unity.Burst.SharedStatic<Unity.Entities.TypeIndex>
	// Unity.Burst.SharedStatic<int>
	// Unity.Collections.FixedList128Bytes.Enumerator<Unity.Collections.FixedString64Bytes>
	// Unity.Collections.FixedList128Bytes.Enumerator<int>
	// Unity.Collections.FixedList128Bytes<Unity.Collections.FixedString64Bytes>
	// Unity.Collections.FixedList128Bytes<int>
	// Unity.Collections.FixedList32Bytes.Enumerator<Unity.Collections.FixedString64Bytes>
	// Unity.Collections.FixedList32Bytes.Enumerator<int>
	// Unity.Collections.FixedList32Bytes<Unity.Collections.FixedString64Bytes>
	// Unity.Collections.FixedList32Bytes<int>
	// Unity.Collections.FixedList4096Bytes<Unity.Collections.FixedString64Bytes>
	// Unity.Collections.FixedList4096Bytes<int>
	// Unity.Collections.FixedList512Bytes.Enumerator<Unity.Collections.FixedString64Bytes>
	// Unity.Collections.FixedList512Bytes.Enumerator<int>
	// Unity.Collections.FixedList512Bytes<Unity.Collections.FixedString64Bytes>
	// Unity.Collections.FixedList512Bytes<int>
	// Unity.Collections.FixedList64Bytes<Unity.Collections.FixedString64Bytes>
	// Unity.Collections.FixedList64Bytes<int>
	// Unity.Collections.IIndexable<byte>
	// Unity.Collections.INativeList<byte>
	// Unity.Collections.LowLevel.Unsafe.HashMapHelper.Enumerator<Unity.Collections.FixedString128Bytes>
	// Unity.Collections.LowLevel.Unsafe.HashMapHelper<Unity.Collections.FixedString128Bytes>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelWriter<Unity.Entities.ComponentType>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelWriter<Unity.Entities.EntityQuery>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelWriter<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList.ParallelWriter<int>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList<Unity.Entities.ComponentType>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList<Unity.Entities.EntityQuery>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// Unity.Collections.LowLevel.Unsafe.UnsafeList<int>
	// Unity.Collections.NativeArray.Enumerator<Main.Buff>
	// Unity.Collections.NativeArray.Enumerator<Main.BuffOld>
	// Unity.Collections.NativeArray.Enumerator<Main.ChaStats>
	// Unity.Collections.NativeArray.Enumerator<Main.DamageInfo>
	// Unity.Collections.NativeArray.Enumerator<Main.GameEvent>
	// Unity.Collections.NativeArray.Enumerator<Main.GameOthersData>
	// Unity.Collections.NativeArray.Enumerator<Main.GlobalConfigData>
	// Unity.Collections.NativeArray.Enumerator<Main.PlayerData>
	// Unity.Collections.NativeArray.Enumerator<Main.PrefabMapData>
	// Unity.Collections.NativeArray.Enumerator<Main.Skill>
	// Unity.Collections.NativeArray.Enumerator<Main.State>
	// Unity.Collections.NativeArray.Enumerator<Main.Trigger>
	// Unity.Collections.NativeArray.Enumerator<Unity.Collections.FixedString128Bytes>
	// Unity.Collections.NativeArray.Enumerator<Unity.Entities.ComponentType>
	// Unity.Collections.NativeArray.Enumerator<Unity.Entities.Entity>
	// Unity.Collections.NativeArray.Enumerator<Unity.Entities.EntityQuery>
	// Unity.Collections.NativeArray.Enumerator<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// Unity.Collections.NativeArray.Enumerator<Unity.Entities.LinkedEntityGroup>
	// Unity.Collections.NativeArray.Enumerator<Unity.Transforms.LocalTransform>
	// Unity.Collections.NativeArray.Enumerator<int>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Main.Buff>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Main.BuffOld>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Main.ChaStats>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Main.DamageInfo>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Main.GameEvent>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Main.GameOthersData>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Main.GlobalConfigData>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Main.PlayerData>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Main.PrefabMapData>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Main.Skill>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Main.State>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Main.Trigger>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Collections.FixedString128Bytes>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Entities.ComponentType>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Entities.Entity>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Entities.EntityQuery>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Entities.LinkedEntityGroup>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<Unity.Transforms.LocalTransform>
	// Unity.Collections.NativeArray.ReadOnly.Enumerator<int>
	// Unity.Collections.NativeArray.ReadOnly<Main.Buff>
	// Unity.Collections.NativeArray.ReadOnly<Main.BuffOld>
	// Unity.Collections.NativeArray.ReadOnly<Main.ChaStats>
	// Unity.Collections.NativeArray.ReadOnly<Main.DamageInfo>
	// Unity.Collections.NativeArray.ReadOnly<Main.GameEvent>
	// Unity.Collections.NativeArray.ReadOnly<Main.GameOthersData>
	// Unity.Collections.NativeArray.ReadOnly<Main.GlobalConfigData>
	// Unity.Collections.NativeArray.ReadOnly<Main.PlayerData>
	// Unity.Collections.NativeArray.ReadOnly<Main.PrefabMapData>
	// Unity.Collections.NativeArray.ReadOnly<Main.Skill>
	// Unity.Collections.NativeArray.ReadOnly<Main.State>
	// Unity.Collections.NativeArray.ReadOnly<Main.Trigger>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Collections.FixedString128Bytes>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Entities.ComponentType>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Entities.Entity>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Entities.EntityQuery>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Entities.LinkedEntityGroup>
	// Unity.Collections.NativeArray.ReadOnly<Unity.Transforms.LocalTransform>
	// Unity.Collections.NativeArray.ReadOnly<int>
	// Unity.Collections.NativeArray<Main.Buff>
	// Unity.Collections.NativeArray<Main.BuffOld>
	// Unity.Collections.NativeArray<Main.ChaStats>
	// Unity.Collections.NativeArray<Main.DamageInfo>
	// Unity.Collections.NativeArray<Main.GameEvent>
	// Unity.Collections.NativeArray<Main.GameOthersData>
	// Unity.Collections.NativeArray<Main.GlobalConfigData>
	// Unity.Collections.NativeArray<Main.PlayerData>
	// Unity.Collections.NativeArray<Main.PrefabMapData>
	// Unity.Collections.NativeArray<Main.Skill>
	// Unity.Collections.NativeArray<Main.State>
	// Unity.Collections.NativeArray<Main.Trigger>
	// Unity.Collections.NativeArray<Unity.Collections.FixedString128Bytes>
	// Unity.Collections.NativeArray<Unity.Entities.ComponentType>
	// Unity.Collections.NativeArray<Unity.Entities.Entity>
	// Unity.Collections.NativeArray<Unity.Entities.EntityQuery>
	// Unity.Collections.NativeArray<Unity.Entities.EntityQueryBuilder.QueryTypes>
	// Unity.Collections.NativeArray<Unity.Entities.LinkedEntityGroup>
	// Unity.Collections.NativeArray<Unity.Transforms.LocalTransform>
	// Unity.Collections.NativeArray<int>
	// Unity.Collections.NativeHashMap.ReadOnly<Unity.Collections.FixedString128Bytes,FMOD.Studio.EventDescription>
	// Unity.Collections.NativeHashMap.ReadOnly<Unity.Collections.FixedString128Bytes,Unity.Entities.Entity>
	// Unity.Collections.NativeHashMap<Unity.Collections.FixedString128Bytes,FMOD.Studio.EventDescription>
	// Unity.Collections.NativeHashMap<Unity.Collections.FixedString128Bytes,Unity.Entities.Entity>
	// Unity.Collections.NativeList.ParallelWriter<int>
	// Unity.Collections.NativeList<int>
	// Unity.Collections.NativeSlice.Enumerator<Main.Buff>
	// Unity.Collections.NativeSlice.Enumerator<Main.BuffOld>
	// Unity.Collections.NativeSlice.Enumerator<Main.DamageInfo>
	// Unity.Collections.NativeSlice.Enumerator<Main.GameEvent>
	// Unity.Collections.NativeSlice.Enumerator<Main.Skill>
	// Unity.Collections.NativeSlice.Enumerator<Main.State>
	// Unity.Collections.NativeSlice.Enumerator<Main.Trigger>
	// Unity.Collections.NativeSlice.Enumerator<Unity.Entities.LinkedEntityGroup>
	// Unity.Collections.NativeSlice<Main.Buff>
	// Unity.Collections.NativeSlice<Main.BuffOld>
	// Unity.Collections.NativeSlice<Main.DamageInfo>
	// Unity.Collections.NativeSlice<Main.GameEvent>
	// Unity.Collections.NativeSlice<Main.Skill>
	// Unity.Collections.NativeSlice<Main.State>
	// Unity.Collections.NativeSlice<Main.Trigger>
	// Unity.Collections.NativeSlice<Unity.Entities.LinkedEntityGroup>
	// Unity.Entities.ComponentLookup<Main.GlobalConfigData>
	// Unity.Entities.ComponentLookup<Main.PrefabMapData>
	// Unity.Entities.ComponentLookup<Unity.Transforms.LocalTransform>
	// Unity.Entities.ComponentTypeHandle<Main.ChaStats>
	// Unity.Entities.ComponentTypeHandle<Main.GameOthersData>
	// Unity.Entities.ComponentTypeHandle<Main.HybridEventData>
	// Unity.Entities.ComponentTypeHandle<Main.PlayerData>
	// Unity.Entities.ComponentTypeHandle<Unity.Transforms.LocalTransform>
	// Unity.Entities.DynamicBuffer<Main.Buff>
	// Unity.Entities.DynamicBuffer<Main.BuffOld>
	// Unity.Entities.DynamicBuffer<Main.DamageInfo>
	// Unity.Entities.DynamicBuffer<Main.GameEvent>
	// Unity.Entities.DynamicBuffer<Main.Skill>
	// Unity.Entities.DynamicBuffer<Main.State>
	// Unity.Entities.DynamicBuffer<Main.Trigger>
	// Unity.Entities.DynamicBuffer<Unity.Entities.LinkedEntityGroup>
	// Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRO<Main.GameOthersData>
	// Unity.Entities.QueryEnumerableWithEntity<Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRO<Main.GameOthersData>>
	// Unity.Entities.RefRO<Main.GameOthersData>
	// Unity.Entities.RefRO<Main.GlobalConfigData>
	// Unity.Entities.RefRO<Main.PrefabMapData>
	// Unity.Entities.RefRO<Unity.Transforms.LocalTransform>
	// Unity.Entities.RefRW<Main.GlobalConfigData>
	// Unity.Entities.RefRW<Main.PrefabMapData>
	// Unity.Entities.RefRW<Unity.Transforms.LocalTransform>
	// Unity.Entities.TypeManager.SharedTypeIndex<object>
	// Unity.Entities.UnityObjectRef<object>
	// UnityEngine.Events.InvokableCall<UnityEngine.Vector2>
	// UnityEngine.Events.InvokableCall<byte>
	// UnityEngine.Events.InvokableCall<float>
	// UnityEngine.Events.InvokableCall<int>
	// UnityEngine.Events.InvokableCall<object>
	// UnityEngine.Events.UnityAction<UnityEngine.Vector2>
	// UnityEngine.Events.UnityAction<byte>
	// UnityEngine.Events.UnityAction<float>
	// UnityEngine.Events.UnityAction<int>
	// UnityEngine.Events.UnityAction<object>
	// UnityEngine.Events.UnityEvent<UnityEngine.Vector2>
	// UnityEngine.Events.UnityEvent<byte>
	// UnityEngine.Events.UnityEvent<float>
	// UnityEngine.Events.UnityEvent<int>
	// UnityEngine.Events.UnityEvent<object>
	// }}

	public void RefMethods()
	{
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_Logic.HotUpdateMain.<GoToUIScene>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_Logic.HotUpdateMain.<GoToUIScene>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_Logic.HotUpdateMain.<InitTypeAndMetaData>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_Logic.HotUpdateMain.<InitTypeAndMetaData>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_Logic.HotUpdateMain.<LoadMetadataForAOTAssemblies>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_Logic.HotUpdateMain.<LoadMetadataForAOTAssemblies>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<DoPlayerPos>d__35>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<DoPlayerPos>d__35&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<EnableGuide>d__36>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<EnableGuide>d__36&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<EnableLoading>d__20>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<EnableLoading>d__20&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<RefreshAllPanelL10N>d__150>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<RefreshAllPanelL10N>d__150&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<SpawnMapElement>d__37>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<SpawnMapElement>d__37&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<SpawnMapElement>d__38>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<SpawnMapElement>d__38&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<TypeWriteEffect>d__94>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<TypeWriteEffect>d__94&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.JsonManager.<SavePlayerData>d__8>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.JsonManager.<SavePlayerData>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.JsonManager.<SaveSharedData>d__12>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.JsonManager.<SaveSharedData>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.DemoEntry.<InitTables>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.DemoEntry.<InitTables>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.ImageComponent.<SetSpriteAsync>d__3<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.ImageComponent.<SetSpriteAsync>d__3<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.ResourcesLoader.<InitAsync>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.ResourcesLoader.<InitAsync>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.Scene.<WaitForCompleted>d__14>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.Scene.<WaitForCompleted>d__14&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.SceneResManager.<UnloadSceneAsync>d__6>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.SceneResManager.<UnloadSceneAsync>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.SceneResManager.<WaitForCompleted>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.SceneResManager.<WaitForCompleted>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UICommon_Prompt.<StartAnimation>d__11>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UICommon_Prompt.<StartAnimation>d__11&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UILoading.<LoadAssets>d__18>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UILoading.<LoadAssets>d__18&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIManager.<EnableLoading>d__28>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIManager.<EnableLoading>d__28&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIManager.<PlayTranstionFX>d__30>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIManager.<PlayTranstionFX>d__30&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Achieve.<ClosePanel>d__37>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Achieve.<ClosePanel>d__37&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Achieve.<InitNode>d__36>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Achieve.<InitNode>d__36&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Activity_Monopoly.<PlayerMoveAnim>d__91>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Activity_Monopoly.<PlayerMoveAnim>d__91&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Activity_Monopoly.<UpdateGrid>d__96>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Activity_Monopoly.<UpdateGrid>d__96&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_AnimTools.<AlphaRefresh>d__19>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_AnimTools.<AlphaRefresh>d__19&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_AnimTools.<ScaleRefresh>d__18>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_AnimTools.<ScaleRefresh>d__18&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_AnimTools.<TranRefresh>d__20>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_AnimTools.<TranRefresh>d__20&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_CompoundDongHua.<GenereteItem>d__28>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_CompoundDongHua.<GenereteItem>d__28&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_JiyuGame.<OnButtonClickAnim>d__74>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_JiyuGame.<OnButtonClickAnim>d__74&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_JiyuGame.<SetEaseEffectForTag5>d__73>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_JiyuGame.<SetEaseEffectForTag5>d__73&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_JiyuGame.<UnLockTag>d__40>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_JiyuGame.<UnLockTag>d__40&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Mail.<ClosePanel>d__40>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Mail.<ClosePanel>d__40&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Main.<PlayTreasureAnim>d__107>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Main.<PlayTreasureAnim>d__107&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Main.<PlayerTipOccurAsyc>d__101>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Main.<PlayerTipOccurAsyc>d__101&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Notice.<ClosePanel>d__30>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Notice.<ClosePanel>d__30&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Notice.<DataInit>d__32>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Notice.<DataInit>d__32&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_RunTimeHUD.<IntroGuideOrder>d__76>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_RunTimeHUD.<IntroGuideOrder>d__76&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_RunTimeHUD.<PlaySpineUIFX>d__70>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_RunTimeHUD.<PlaySpineUIFX>d__70&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_RunTimeHUD.<SpawnEnemy>d__79>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_RunTimeHUD.<SpawnEnemy>d__79&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Settings.<ClosePanel>d__47>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Settings.<ClosePanel>d__47&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_Guid.<AlphaRefresh>d__41>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_Guid.<AlphaRefresh>d__41&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_Guid.<ScaleRefresh>d__40>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_Guid.<ScaleRefresh>d__40&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_Guid.<TranRefresh>d__42>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_Guid.<TranRefresh>d__42&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.YooSceneLoader.<UnloadSceneAsync>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.YooSceneLoader.<UnloadSceneAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.YooSceneLoader.<WaitForCompleted>d__5>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.YooSceneLoader.<WaitForCompleted>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<Unity.Entities.Entity>,XFramework.UIPanel_RunTimeHUD.<IntroGuideOrder>d__76>(Cysharp.Threading.Tasks.UniTask.Awaiter<Unity.Entities.Entity>&,XFramework.UIPanel_RunTimeHUD.<IntroGuideOrder>d__76&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,XFramework.UILoading.<DoFillAmount>d__20>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,XFramework.UILoading.<DoFillAmount>d__20&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,XFramework.UILoading.<LoadAssets>d__18>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,XFramework.UILoading.<LoadAssets>d__18&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,XFramework.UIPanel_CompoundDongHua.<GenereteItem>d__28>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,XFramework.UIPanel_CompoundDongHua.<GenereteItem>d__28&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,XFramework.UIPanel_RunTimeHUD.<IntroGuideOrder>d__76>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,XFramework.UIPanel_RunTimeHUD.<IntroGuideOrder>d__76&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornSceneHelper.<InitInputPrefab>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornSceneHelper.<InitInputPrefab>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<DownloadFileByUrl>d__135>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<DownloadFileByUrl>d__135&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<DownloadNotice>d__133>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<DownloadNotice>d__133&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<InitBlur>d__17>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<InitBlur>d__17&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<LoadImage>d__136>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<LoadImage>d__136&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.AudioManager.<LoadAudio>d__18>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.AudioManager.<LoadAudio>d__18&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.DemoEntry.<InitTables>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.DemoEntry.<InitTables>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.IntroGuide.<InitInputPrefab>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.IntroGuide.<InitInputPrefab>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.LoadingScene.<WaitForCompleted>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.LoadingScene.<WaitForCompleted>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.RunTimeScene.<InitInputPrefab>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.RunTimeScene.<InitInputPrefab>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UICommon_Label.<InitBag>d__25>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UICommon_Label.<InitBag>d__25&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UICommon_Label.<InitEquip>d__26>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UICommon_Label.<InitEquip>d__26&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UICommon_Label.<InitEquipCompound>d__27>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UICommon_Label.<InitEquipCompound>d__27&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UICommon_Label.<InitEquipCompoundSelected>d__30>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UICommon_Label.<InitEquipCompoundSelected>d__30&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UICommon_Label.<InitMaterial>d__32>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UICommon_Label.<InitMaterial>d__32&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UICommon_Reward.<InitRewardItems>d__16>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UICommon_Reward.<InitRewardItems>d__16&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIImageExtensions.<SetSpriteAsync>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIImageExtensions.<SetSpriteAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UILoading.<LoadObjectAsync>d__19>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UILoading.<LoadObjectAsync>d__19&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Achieve.<CreateGroup>d__46>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Achieve.<CreateGroup>d__46&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Activity_Monopoly.<UpdateGrid>d__96>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Activity_Monopoly.<UpdateGrid>d__96&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_CompoundDongHua.<InitRewardItems>d__35>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_CompoundDongHua.<InitRewardItems>d__35&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_JiyuGame.<CreateTagPanel>d__46>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_JiyuGame.<CreateTagPanel>d__46&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_JiyuGame.<InitPanel>d__37>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_JiyuGame.<InitPanel>d__37&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Mail.<InitMailPanel>d__47>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Mail.<InitMailPanel>d__47&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Main.<IntroGuideOrder>d__92>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Main.<IntroGuideOrder>d__92&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_RunTimeHUD.<IntroGuideOrder>d__76>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_RunTimeHUD.<IntroGuideOrder>d__76&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UISubPanel_Equipment.<Init>d__17>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UISubPanel_Equipment.<Init>d__17&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UISubPanel_Shop_1403_Fund.<Module1403_Help_CreateOneRowItem>d__53>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UISubPanel_Shop_1403_Fund.<Module1403_Help_CreateOneRowItem>d__53&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UISubPanel_Shop_Pre.<PreInit>d__23>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UISubPanel_Shop_Pre.<PreInit>d__23&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<object,XFramework.LoadingScene.<WaitForCompleted>d__3>(object&,XFramework.LoadingScene.<WaitForCompleted>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<object,XFramework.UILoading.<LoadAssets>d__18>(object&,XFramework.UILoading.<LoadAssets>d__18&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<ChangeSoftness>d__13>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<ChangeSoftness>d__13&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<PlayUIImageSweepFX>d__23>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<PlayUIImageSweepFX>d__23&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<PlayUIImageTranstionFX>d__22>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<PlayUIImageTranstionFX>d__22&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosB2U>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosB2U>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosLtoR>d__6>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosLtoR>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosRtoL>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosRtoL>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosUtoB>d__5>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosUtoB>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndScale>d__8>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndScale>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndScaleWithFour>d__10>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndScaleWithFour>d__10&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<SetScaleWithBounce>d__11>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<SetScaleWithBounce>d__11&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Activity_Monopoly.<DoScaleAnim>d__94>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Activity_Monopoly.<DoScaleAnim>d__94&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Compound.<Effect1>d__42>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Compound.<Effect1>d__42&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Compound.<Effect2>d__41>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Compound.<Effect2>d__41&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Compound.<SetEffect>d__40>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Compound.<SetEffect>d__40&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<Cysharp.Threading.Tasks.AsyncUnit>,HotFix_UI.UnicornUIHelper.<SetScaleWithBounceClose>d__12>(Cysharp.Threading.Tasks.UniTask.Awaiter<Cysharp.Threading.Tasks.AsyncUnit>&,HotFix_UI.UnicornUIHelper.<SetScaleWithBounceClose>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Unity.Entities.Entity>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<DoMonsterPos>d__33>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<DoMonsterPos>d__33&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_CompoundDongHua.<IsGetData>d__33>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_CompoundDongHua.<IsGetData>d__33&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_RunTimeHUD.<CheckEnemyClear>d__71>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_RunTimeHUD.<CheckEnemyClear>d__71&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_RunTimeHUD.<CheckQueryExist>d__72>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_RunTimeHUD.<CheckQueryExist>d__72&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<float>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIContainerBoxBar.<CreateReward>d__36>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIContainerBoxBar.<CreateReward>d__36&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Challege.<UpdateFromCurrentMainID>d__74>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Challege.<UpdateFromCurrentMainID>d__74&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<CaptureScreenAsync>d__16>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<CaptureScreenAsync>d__16&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<CaptureScreenshot>d__18>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<CaptureScreenshot>d__18&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.JsonManager.<LoadPlayerData>d__9>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.JsonManager.<LoadPlayerData>d__9&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.JsonManager.<LoadSharedData>d__11>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.JsonManager.<LoadSharedData>d__11&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.DemoEntry.<Loader>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.DemoEntry.<Loader>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_CompoundDongHua.<setScaleCanCancel>d__36>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_CompoundDongHua.<setScaleCanCancel>d__36&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.YooResourcesLoader.<InstantiateAsync>d__10>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.YooResourcesLoader.<InstantiateAsync>d__10&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.YooResourcesLoader.<InstantiateAsync>d__11>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.YooResourcesLoader.<InstantiateAsync>d__11&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.YooResourcesLoader.<InstantiateAsync>d__12>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.YooResourcesLoader.<InstantiateAsync>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.YooResourcesLoader.<InstantiateAsync>d__9>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.YooResourcesLoader.<InstantiateAsync>d__9&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.YooResourcesLoader.<LoadAssetAsync>d__3<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.YooResourcesLoader.<LoadAssetAsync>d__3<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.YooResourcesLoader.<LoadAssetAsync>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.YooResourcesLoader.<LoadAssetAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.JsonManager.<LoadPlayerData>d__10>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.JsonManager.<LoadPlayerData>d__10&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.JsonManager.<LoadPlayerData>d__9>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.JsonManager.<LoadPlayerData>d__9&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.JsonManager.<LoadSharedData>d__11>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.JsonManager.<LoadSharedData>d__11&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.ResourcesManager.<InstantiateAsync>d__16>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.ResourcesManager.<InstantiateAsync>d__16&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.ResourcesManager.<InstantiateAsync>d__17>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.ResourcesManager.<InstantiateAsync>d__17&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.ResourcesManager.<InstantiateAsync>d__20>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.ResourcesManager.<InstantiateAsync>d__20&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.ResourcesManager.<InstantiateAsync>d__21>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.ResourcesManager.<InstantiateAsync>d__21&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.ResourcesManager.<LoadAssetAsync>d__10<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.ResourcesManager.<LoadAssetAsync>d__10<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.ResourcesManager.<LoadAssetAsync>d__11<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.ResourcesManager.<LoadAssetAsync>d__11<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.ResourcesManager.<LoadAssetAsync>d__12>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.ResourcesManager.<LoadAssetAsync>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIHelper.<CreateAsync>d__10<object,object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIHelper.<CreateAsync>d__10<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIHelper.<CreateAsync>d__12>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIHelper.<CreateAsync>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIHelper.<CreateAsync>d__13<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIHelper.<CreateAsync>d__13<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIHelper.<CreateAsync>d__14<object,object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIHelper.<CreateAsync>d__14<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIHelper.<CreateAsync>d__15>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIHelper.<CreateAsync>d__15&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIHelper.<CreateAsync>d__16<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIHelper.<CreateAsync>d__16<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIHelper.<CreateAsync>d__17<object,object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIHelper.<CreateAsync>d__17<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIHelper.<CreateAsync>d__8>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIHelper.<CreateAsync>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIHelper.<CreateAsync>d__9<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIHelper.<CreateAsync>d__9<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIHelper.<CreateAsyncNew>d__6<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIHelper.<CreateAsyncNew>d__6<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIHelper.<CreateAsyncNew>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIHelper.<CreateAsyncNew>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIHelper.<CreateAsyncWithPrefabKey>d__11<object,object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIHelper.<CreateAsyncWithPrefabKey>d__11<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIHelper.<CreateOverLayTipsAsync>d__18<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIHelper.<CreateOverLayTipsAsync>d__18<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIListComponent.<CreateWithKeyAsync>d__44<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIListComponent.<CreateWithKeyAsync>d__44<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIListComponent.<CreateWithKeyAsync>d__45<object,object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIListComponent.<CreateWithKeyAsync>d__45<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIListComponent.<CreateWithKeyAsync>d__46>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIListComponent.<CreateWithKeyAsync>d__46&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIListComponent.<CreateWithUITypeAsync>d__39>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIListComponent.<CreateWithUITypeAsync>d__39&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIListComponent.<CreateWithUITypeAsync>d__40<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIListComponent.<CreateWithUITypeAsync>d__40<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIListComponent.<CreateWithUITypeAsync>d__41>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIListComponent.<CreateWithUITypeAsync>d__41&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIListComponent.<CreateWithUITypeAsync>d__42<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIListComponent.<CreateWithUITypeAsync>d__42<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIListComponent.<CreateWithUITypeAsync>d__43<object,object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIListComponent.<CreateWithUITypeAsync>d__43<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIListComponent.<InnerCreateWithKeyAsync>d__30<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIListComponent.<InnerCreateWithKeyAsync>d__30<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIListComponent.<InnerCreateWithUITypeAsync>d__28>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIListComponent.<InnerCreateWithUITypeAsync>d__28&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIListComponent.<InnerCreateWithUITypeAsync>d__29>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIListComponent.<InnerCreateWithUITypeAsync>d__29&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIManager.<CreateAsync>d__52>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIManager.<CreateAsync>d__52&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIManager.<CreateAsync>d__53<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIManager.<CreateAsync>d__53<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIManager.<CreateAsync>d__54<object,object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIManager.<CreateAsync>d__54<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIManager.<CreateAsync>d__55<object,object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIManager.<CreateAsync>d__55<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIManager.<CreateAsync>d__56>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIManager.<CreateAsync>d__56&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIManager.<CreateAsync>d__57<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIManager.<CreateAsync>d__57<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIManager.<CreateAsync>d__58<object,object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIManager.<CreateAsync>d__58<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIManager.<CreateAsync>d__59>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIManager.<CreateAsync>d__59&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIManager.<CreateAsync>d__60<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIManager.<CreateAsync>d__60<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIManager.<CreateAsync>d__61<object,object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIManager.<CreateAsync>d__61<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIManager.<CreateAsyncNew>d__50<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIManager.<CreateAsyncNew>d__50<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIManager.<CreateAsyncNew>d__51>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIManager.<CreateAsyncNew>d__51&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIManager.<CreateInnerAsync>d__39>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIManager.<CreateInnerAsync>d__39&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIManager.<CreateInnerAsync>d__40>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIManager.<CreateInnerAsync>d__40&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIManager.<GetGameObjectAsync>d__41>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIManager.<GetGameObjectAsync>d__41&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIManager.<GetGameObjectAsync>d__42>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIManager.<GetGameObjectAsync>d__42&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Challege.<CreateReward>d__85>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Challege.<CreateReward>d__85&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<HotFix_Logic.HotUpdateMain.<GoToUIScene>d__7>(HotFix_Logic.HotUpdateMain.<GoToUIScene>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<HotFix_Logic.HotUpdateMain.<InitTypeAndMetaData>d__4>(HotFix_Logic.HotUpdateMain.<InitTypeAndMetaData>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<HotFix_Logic.HotUpdateMain.<LoadMetadataForAOTAssemblies>d__3>(HotFix_Logic.HotUpdateMain.<LoadMetadataForAOTAssemblies>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<HotFix_UI.UnicornSceneHelper.<InitInputPrefab>d__4>(HotFix_UI.UnicornSceneHelper.<InitInputPrefab>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<DoPlayerPos>d__35>(HotFix_UI.UnicornUIHelper.<DoPlayerPos>d__35&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<DownloadFileByUrl>d__135>(HotFix_UI.UnicornUIHelper.<DownloadFileByUrl>d__135&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<DownloadNotice>d__133>(HotFix_UI.UnicornUIHelper.<DownloadNotice>d__133&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<DownloadShare>d__134>(HotFix_UI.UnicornUIHelper.<DownloadShare>d__134&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<EnableGuide>d__36>(HotFix_UI.UnicornUIHelper.<EnableGuide>d__36&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<EnableLoading>d__20>(HotFix_UI.UnicornUIHelper.<EnableLoading>d__20&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<InitBlur>d__17>(HotFix_UI.UnicornUIHelper.<InitBlur>d__17&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<LoadImage>d__136>(HotFix_UI.UnicornUIHelper.<LoadImage>d__136&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<RefreshAllPanelL10N>d__150>(HotFix_UI.UnicornUIHelper.<RefreshAllPanelL10N>d__150&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<SpawnMapElement>d__37>(HotFix_UI.UnicornUIHelper.<SpawnMapElement>d__37&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<SpawnMapElement>d__38>(HotFix_UI.UnicornUIHelper.<SpawnMapElement>d__38&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<TypeWriteEffect>d__94>(HotFix_UI.UnicornUIHelper.<TypeWriteEffect>d__94&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<HotFix_UI.JsonManager.<SavePlayerData>d__8>(HotFix_UI.JsonManager.<SavePlayerData>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<HotFix_UI.JsonManager.<SaveSharedData>d__12>(HotFix_UI.JsonManager.<SaveSharedData>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.AudioManager.<LoadAudio>d__18>(XFramework.AudioManager.<LoadAudio>d__18&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.DemoEntry.<InitTables>d__1>(XFramework.DemoEntry.<InitTables>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.ImageComponent.<SetSpriteAsync>d__3<object>>(XFramework.ImageComponent.<SetSpriteAsync>d__3<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.IntroGuide.<InitInputPrefab>d__3>(XFramework.IntroGuide.<InitInputPrefab>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.LoadingScene.<WaitForCompleted>d__3>(XFramework.LoadingScene.<WaitForCompleted>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.ResourcesLoader.<InitAsync>d__0>(XFramework.ResourcesLoader.<InitAsync>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.RunTimeScene.<InitInputPrefab>d__2>(XFramework.RunTimeScene.<InitInputPrefab>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.Scene.<WaitForCompleted>d__14>(XFramework.Scene.<WaitForCompleted>d__14&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.SceneResManager.<UnloadSceneAsync>d__6>(XFramework.SceneResManager.<UnloadSceneAsync>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.SceneResManager.<WaitForCompleted>d__7>(XFramework.SceneResManager.<WaitForCompleted>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UICommon_Label.<InitBag>d__25>(XFramework.UICommon_Label.<InitBag>d__25&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UICommon_Label.<InitEquip>d__26>(XFramework.UICommon_Label.<InitEquip>d__26&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UICommon_Label.<InitEquipCompound>d__27>(XFramework.UICommon_Label.<InitEquipCompound>d__27&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UICommon_Label.<InitEquipCompoundSelected>d__30>(XFramework.UICommon_Label.<InitEquipCompoundSelected>d__30&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UICommon_Label.<InitMaterial>d__32>(XFramework.UICommon_Label.<InitMaterial>d__32&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UICommon_Prompt.<StartAnimation>d__11>(XFramework.UICommon_Prompt.<StartAnimation>d__11&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UICommon_Reward.<InitRewardItems>d__16>(XFramework.UICommon_Reward.<InitRewardItems>d__16&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIImageExtensions.<SetSpriteAsync>d__4>(XFramework.UIImageExtensions.<SetSpriteAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UILoading.<DoFillAmount>d__20>(XFramework.UILoading.<DoFillAmount>d__20&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UILoading.<LoadAssets>d__18>(XFramework.UILoading.<LoadAssets>d__18&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UILoading.<LoadObjectAsync>d__19>(XFramework.UILoading.<LoadObjectAsync>d__19&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIManager.<EnableLoading>d__28>(XFramework.UIManager.<EnableLoading>d__28&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIManager.<PlayTranstionFX>d__30>(XFramework.UIManager.<PlayTranstionFX>d__30&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_Achieve.<ClosePanel>d__37>(XFramework.UIPanel_Achieve.<ClosePanel>d__37&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_Achieve.<CreateGroup>d__46>(XFramework.UIPanel_Achieve.<CreateGroup>d__46&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_Achieve.<InitNode>d__36>(XFramework.UIPanel_Achieve.<InitNode>d__36&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_Activity_Monopoly.<PlayerMoveAnim>d__91>(XFramework.UIPanel_Activity_Monopoly.<PlayerMoveAnim>d__91&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_Activity_Monopoly.<UpdateGrid>d__96>(XFramework.UIPanel_Activity_Monopoly.<UpdateGrid>d__96&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_AnimTools.<AlphaRefresh>d__19>(XFramework.UIPanel_AnimTools.<AlphaRefresh>d__19&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_AnimTools.<ScaleRefresh>d__18>(XFramework.UIPanel_AnimTools.<ScaleRefresh>d__18&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_AnimTools.<TranRefresh>d__20>(XFramework.UIPanel_AnimTools.<TranRefresh>d__20&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_BattleInfo.<CreateDamageBar>d__63>(XFramework.UIPanel_BattleInfo.<CreateDamageBar>d__63&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_CompoundDongHua.<GenereteItem>d__28>(XFramework.UIPanel_CompoundDongHua.<GenereteItem>d__28&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_CompoundDongHua.<InitRewardItems>d__35>(XFramework.UIPanel_CompoundDongHua.<InitRewardItems>d__35&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_JiyuGame.<CreateTagPanel>d__46>(XFramework.UIPanel_JiyuGame.<CreateTagPanel>d__46&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_JiyuGame.<InitPanel>d__37>(XFramework.UIPanel_JiyuGame.<InitPanel>d__37&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_JiyuGame.<OnButtonClickAnim>d__74>(XFramework.UIPanel_JiyuGame.<OnButtonClickAnim>d__74&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_JiyuGame.<ReCreateAllTagPanel>d__45>(XFramework.UIPanel_JiyuGame.<ReCreateAllTagPanel>d__45&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_JiyuGame.<SetEaseEffectForTag5>d__73>(XFramework.UIPanel_JiyuGame.<SetEaseEffectForTag5>d__73&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_JiyuGame.<UnLockTag>d__40>(XFramework.UIPanel_JiyuGame.<UnLockTag>d__40&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_Mail.<ClosePanel>d__40>(XFramework.UIPanel_Mail.<ClosePanel>d__40&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_Mail.<InitMailPanel>d__47>(XFramework.UIPanel_Mail.<InitMailPanel>d__47&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_Main.<IntroGuideOrder>d__92>(XFramework.UIPanel_Main.<IntroGuideOrder>d__92&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_Main.<PlayTreasureAnim>d__107>(XFramework.UIPanel_Main.<PlayTreasureAnim>d__107&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_Main.<PlayerTipOccurAsyc>d__101>(XFramework.UIPanel_Main.<PlayerTipOccurAsyc>d__101&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_Notice.<ClosePanel>d__30>(XFramework.UIPanel_Notice.<ClosePanel>d__30&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_Notice.<DataInit>d__32>(XFramework.UIPanel_Notice.<DataInit>d__32&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_Patrol.<ClosePanel>d__72>(XFramework.UIPanel_Patrol.<ClosePanel>d__72&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_RunTimeHUD.<IntroGuideOrder>d__76>(XFramework.UIPanel_RunTimeHUD.<IntroGuideOrder>d__76&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_RunTimeHUD.<PlaySpineUIFX>d__70>(XFramework.UIPanel_RunTimeHUD.<PlaySpineUIFX>d__70&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_RunTimeHUD.<SpawnEnemy>d__79>(XFramework.UIPanel_RunTimeHUD.<SpawnEnemy>d__79&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_Settings.<ClosePanel>d__47>(XFramework.UIPanel_Settings.<ClosePanel>d__47&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UIPanel_Sweep.<ClosePanel>d__66>(XFramework.UIPanel_Sweep.<ClosePanel>d__66&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UISubPanel_Equipment.<Init>d__17>(XFramework.UISubPanel_Equipment.<Init>d__17&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UISubPanel_Guid.<AlphaRefresh>d__41>(XFramework.UISubPanel_Guid.<AlphaRefresh>d__41&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UISubPanel_Guid.<ScaleRefresh>d__40>(XFramework.UISubPanel_Guid.<ScaleRefresh>d__40&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UISubPanel_Guid.<TranRefresh>d__42>(XFramework.UISubPanel_Guid.<TranRefresh>d__42&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UISubPanel_Shop_1403_Fund.<Module1403_Help_CreateOneRowItem>d__53>(XFramework.UISubPanel_Shop_1403_Fund.<Module1403_Help_CreateOneRowItem>d__53&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.UISubPanel_Shop_Pre.<PreInit>d__23>(XFramework.UISubPanel_Shop_Pre.<PreInit>d__23&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.YooResourcesLoader.<InitAsync>d__0>(XFramework.YooResourcesLoader.<InitAsync>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.YooSceneLoader.<UnloadSceneAsync>d__4>(XFramework.YooSceneLoader.<UnloadSceneAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<XFramework.YooSceneLoader.<WaitForCompleted>d__5>(XFramework.YooSceneLoader.<WaitForCompleted>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.Start<HotFix_UI.UnicornUIHelper.<ChangeSoftness>d__13>(HotFix_UI.UnicornUIHelper.<ChangeSoftness>d__13&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.Start<HotFix_UI.UnicornUIHelper.<PlayUIImageSweepFX>d__23>(HotFix_UI.UnicornUIHelper.<PlayUIImageSweepFX>d__23&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.Start<HotFix_UI.UnicornUIHelper.<PlayUIImageTranstionFX>d__22>(HotFix_UI.UnicornUIHelper.<PlayUIImageTranstionFX>d__22&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.Start<HotFix_UI.UnicornUIHelper.<SetAngleRotate>d__1>(HotFix_UI.UnicornUIHelper.<SetAngleRotate>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.Start<HotFix_UI.UnicornUIHelper.<SetAngleRotateXZ>d__3>(HotFix_UI.UnicornUIHelper.<SetAngleRotateXZ>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.Start<HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosB2U>d__4>(HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosB2U>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.Start<HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosLtoR>d__6>(HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosLtoR>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.Start<HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosRtoL>d__7>(HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosRtoL>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.Start<HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosUtoB>d__5>(HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndPosUtoB>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.Start<HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndScale>d__8>(HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndScale>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.Start<HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndScaleWithBounce>d__9>(HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndScaleWithBounce>d__9&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.Start<HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndScaleWithFour>d__10>(HotFix_UI.UnicornUIHelper.<SetEaseAlphaAndScaleWithFour>d__10&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.Start<HotFix_UI.UnicornUIHelper.<SetScaleWithBounce>d__11>(HotFix_UI.UnicornUIHelper.<SetScaleWithBounce>d__11&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.Start<HotFix_UI.UnicornUIHelper.<SetScaleWithBounceClose>d__12>(HotFix_UI.UnicornUIHelper.<SetScaleWithBounceClose>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.Start<XFramework.UIPanel_Activity_Monopoly.<DoScaleAnim>d__94>(XFramework.UIPanel_Activity_Monopoly.<DoScaleAnim>d__94&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.Start<XFramework.UIPanel_Compound.<Effect1>d__42>(XFramework.UIPanel_Compound.<Effect1>d__42&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.Start<XFramework.UIPanel_Compound.<Effect2>d__41>(XFramework.UIPanel_Compound.<Effect2>d__41&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Cysharp.Threading.Tasks.AsyncUnit>.Start<XFramework.UIPanel_Compound.<SetEffect>d__40>(XFramework.UIPanel_Compound.<SetEffect>d__40&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<Unity.Entities.Entity>.Start<HotFix_UI.UnicornUIHelper.<DoMonsterPos>d__33>(HotFix_UI.UnicornUIHelper.<DoMonsterPos>d__33&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.Start<XFramework.UIPanel_CompoundDongHua.<IsGetData>d__33>(XFramework.UIPanel_CompoundDongHua.<IsGetData>d__33&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.Start<XFramework.UIPanel_RunTimeHUD.<CheckEnemyClear>d__71>(XFramework.UIPanel_RunTimeHUD.<CheckEnemyClear>d__71&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.Start<XFramework.UIPanel_RunTimeHUD.<CheckQueryExist>d__72>(XFramework.UIPanel_RunTimeHUD.<CheckQueryExist>d__72&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<float>.Start<XFramework.UIContainerBoxBar.<CreateReward>d__36>(XFramework.UIContainerBoxBar.<CreateReward>d__36&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<int>.Start<XFramework.UIPanel_Challege.<UpdateFromCurrentMainID>d__74>(XFramework.UIPanel_Challege.<UpdateFromCurrentMainID>d__74&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<HotFix_UI.UnicornUIHelper.<CaptureScreenAsync>d__16>(HotFix_UI.UnicornUIHelper.<CaptureScreenAsync>d__16&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<HotFix_UI.UnicornUIHelper.<CaptureScreenshot>d__18>(HotFix_UI.UnicornUIHelper.<CaptureScreenshot>d__18&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<HotFix_UI.UnicornUIHelper.<SetBookmarkSwing>d__2>(HotFix_UI.UnicornUIHelper.<SetBookmarkSwing>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<HotFix_UI.JsonManager.<LoadPlayerData>d__10>(HotFix_UI.JsonManager.<LoadPlayerData>d__10&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<HotFix_UI.JsonManager.<LoadPlayerData>d__9>(HotFix_UI.JsonManager.<LoadPlayerData>d__9&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<HotFix_UI.JsonManager.<LoadSharedData>d__11>(HotFix_UI.JsonManager.<LoadSharedData>d__11&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.DemoEntry.<Loader>d__3>(XFramework.DemoEntry.<Loader>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.ResourcesManager.<InstantiateAsync>d__16>(XFramework.ResourcesManager.<InstantiateAsync>d__16&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.ResourcesManager.<InstantiateAsync>d__17>(XFramework.ResourcesManager.<InstantiateAsync>d__17&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.ResourcesManager.<InstantiateAsync>d__20>(XFramework.ResourcesManager.<InstantiateAsync>d__20&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.ResourcesManager.<InstantiateAsync>d__21>(XFramework.ResourcesManager.<InstantiateAsync>d__21&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.ResourcesManager.<LoadAssetAsync>d__10<object>>(XFramework.ResourcesManager.<LoadAssetAsync>d__10<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.ResourcesManager.<LoadAssetAsync>d__11<object>>(XFramework.ResourcesManager.<LoadAssetAsync>d__11<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.ResourcesManager.<LoadAssetAsync>d__12>(XFramework.ResourcesManager.<LoadAssetAsync>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__10<object,int>>(XFramework.UIHelper.<CreateAsync>d__10<object,int>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__10<object,object>>(XFramework.UIHelper.<CreateAsync>d__10<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__12>(XFramework.UIHelper.<CreateAsync>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__13<UnityEngine.Vector3>>(XFramework.UIHelper.<CreateAsync>d__13<UnityEngine.Vector3>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__13<XFramework.UICommon_ItemTips.TipsData>>(XFramework.UIHelper.<CreateAsync>d__13<XFramework.UICommon_ItemTips.TipsData>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__13<int>>(XFramework.UIHelper.<CreateAsync>d__13<int>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__13<object>>(XFramework.UIHelper.<CreateAsync>d__13<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__14<UnityEngine.Vector3,UnityEngine.Vector3>>(XFramework.UIHelper.<CreateAsync>d__14<UnityEngine.Vector3,UnityEngine.Vector3>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__14<object,object>>(XFramework.UIHelper.<CreateAsync>d__14<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__15>(XFramework.UIHelper.<CreateAsync>d__15&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__16<UnityEngine.Vector2>>(XFramework.UIHelper.<CreateAsync>d__16<UnityEngine.Vector2>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__16<UnityEngine.Vector3>>(XFramework.UIHelper.<CreateAsync>d__16<UnityEngine.Vector3>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__16<XFramework.CompoundSucData>>(XFramework.UIHelper.<CreateAsync>d__16<XFramework.CompoundSucData>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__16<XFramework.UIBagItem.BagItemData>>(XFramework.UIHelper.<CreateAsync>d__16<XFramework.UIBagItem.BagItemData>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__16<XFramework.UICommon_Btn1.Parameter>>(XFramework.UIHelper.<CreateAsync>d__16<XFramework.UICommon_Btn1.Parameter>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__16<XFramework.UICommon_ItemTips.ItemTipsData>>(XFramework.UIHelper.<CreateAsync>d__16<XFramework.UICommon_ItemTips.ItemTipsData>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__16<XFramework.UICommon_ItemTips.TipsData>>(XFramework.UIHelper.<CreateAsync>d__16<XFramework.UICommon_ItemTips.TipsData>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__16<int>>(XFramework.UIHelper.<CreateAsync>d__16<int>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__16<object>>(XFramework.UIHelper.<CreateAsync>d__16<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__17<UnityEngine.Vector3,UnityEngine.Vector3>>(XFramework.UIHelper.<CreateAsync>d__17<UnityEngine.Vector3,UnityEngine.Vector3>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__17<int,float>>(XFramework.UIHelper.<CreateAsync>d__17<int,float>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__17<int,int>>(XFramework.UIHelper.<CreateAsync>d__17<int,int>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__17<object,int>>(XFramework.UIHelper.<CreateAsync>d__17<object,int>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__17<object,object>>(XFramework.UIHelper.<CreateAsync>d__17<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__8>(XFramework.UIHelper.<CreateAsync>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__9<UnityEngine.Vector3>>(XFramework.UIHelper.<CreateAsync>d__9<UnityEngine.Vector3>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__9<int>>(XFramework.UIHelper.<CreateAsync>d__9<int>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsync>d__9<object>>(XFramework.UIHelper.<CreateAsync>d__9<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsyncNew>d__6<object>>(XFramework.UIHelper.<CreateAsyncNew>d__6<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsyncNew>d__7>(XFramework.UIHelper.<CreateAsyncNew>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsyncWithPrefabKey>d__11<object,int>>(XFramework.UIHelper.<CreateAsyncWithPrefabKey>d__11<object,int>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateAsyncWithPrefabKey>d__11<object,object>>(XFramework.UIHelper.<CreateAsyncWithPrefabKey>d__11<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIHelper.<CreateOverLayTipsAsync>d__18<object>>(XFramework.UIHelper.<CreateOverLayTipsAsync>d__18<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIListComponent.<CreateWithKeyAsync>d__44<object>>(XFramework.UIListComponent.<CreateWithKeyAsync>d__44<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIListComponent.<CreateWithKeyAsync>d__45<object,object>>(XFramework.UIListComponent.<CreateWithKeyAsync>d__45<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIListComponent.<CreateWithKeyAsync>d__46>(XFramework.UIListComponent.<CreateWithKeyAsync>d__46&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIListComponent.<CreateWithUITypeAsync>d__39>(XFramework.UIListComponent.<CreateWithUITypeAsync>d__39&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIListComponent.<CreateWithUITypeAsync>d__40<object>>(XFramework.UIListComponent.<CreateWithUITypeAsync>d__40<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIListComponent.<CreateWithUITypeAsync>d__41>(XFramework.UIListComponent.<CreateWithUITypeAsync>d__41&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIListComponent.<CreateWithUITypeAsync>d__42<Unity.Mathematics.int3>>(XFramework.UIListComponent.<CreateWithUITypeAsync>d__42<Unity.Mathematics.int3>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIListComponent.<CreateWithUITypeAsync>d__42<UnityEngine.Vector3>>(XFramework.UIListComponent.<CreateWithUITypeAsync>d__42<UnityEngine.Vector3>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIListComponent.<CreateWithUITypeAsync>d__42<XFramework.UIContainer_Bar.ParamterBtn>>(XFramework.UIListComponent.<CreateWithUITypeAsync>d__42<XFramework.UIContainer_Bar.ParamterBtn>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIListComponent.<CreateWithUITypeAsync>d__42<int>>(XFramework.UIListComponent.<CreateWithUITypeAsync>d__42<int>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIListComponent.<CreateWithUITypeAsync>d__42<object>>(XFramework.UIListComponent.<CreateWithUITypeAsync>d__42<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIListComponent.<CreateWithUITypeAsync>d__43<int,byte>>(XFramework.UIListComponent.<CreateWithUITypeAsync>d__43<int,byte>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIListComponent.<CreateWithUITypeAsync>d__43<int,int>>(XFramework.UIListComponent.<CreateWithUITypeAsync>d__43<int,int>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIListComponent.<CreateWithUITypeAsync>d__43<object,int>>(XFramework.UIListComponent.<CreateWithUITypeAsync>d__43<object,int>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIListComponent.<CreateWithUITypeAsync>d__43<object,object>>(XFramework.UIListComponent.<CreateWithUITypeAsync>d__43<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIListComponent.<InnerCreateWithKeyAsync>d__30<object>>(XFramework.UIListComponent.<InnerCreateWithKeyAsync>d__30<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIListComponent.<InnerCreateWithUITypeAsync>d__28>(XFramework.UIListComponent.<InnerCreateWithUITypeAsync>d__28&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIListComponent.<InnerCreateWithUITypeAsync>d__29>(XFramework.UIListComponent.<InnerCreateWithUITypeAsync>d__29&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIManager.<CreateAsync>d__52>(XFramework.UIManager.<CreateAsync>d__52&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIManager.<CreateAsync>d__53<object>>(XFramework.UIManager.<CreateAsync>d__53<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIManager.<CreateAsync>d__54<object,object>>(XFramework.UIManager.<CreateAsync>d__54<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIManager.<CreateAsync>d__55<object,object>>(XFramework.UIManager.<CreateAsync>d__55<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIManager.<CreateAsync>d__56>(XFramework.UIManager.<CreateAsync>d__56&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIManager.<CreateAsync>d__57<object>>(XFramework.UIManager.<CreateAsync>d__57<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIManager.<CreateAsync>d__58<object,object>>(XFramework.UIManager.<CreateAsync>d__58<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIManager.<CreateAsync>d__59>(XFramework.UIManager.<CreateAsync>d__59&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIManager.<CreateAsync>d__60<object>>(XFramework.UIManager.<CreateAsync>d__60<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIManager.<CreateAsync>d__61<object,object>>(XFramework.UIManager.<CreateAsync>d__61<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIManager.<CreateAsyncNew>d__50<object>>(XFramework.UIManager.<CreateAsyncNew>d__50<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIManager.<CreateAsyncNew>d__51>(XFramework.UIManager.<CreateAsyncNew>d__51&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIManager.<CreateInnerAsync>d__39>(XFramework.UIManager.<CreateInnerAsync>d__39&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIManager.<CreateInnerAsync>d__40>(XFramework.UIManager.<CreateInnerAsync>d__40&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIManager.<GetGameObjectAsync>d__41>(XFramework.UIManager.<GetGameObjectAsync>d__41&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIManager.<GetGameObjectAsync>d__42>(XFramework.UIManager.<GetGameObjectAsync>d__42&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIPanel_Challege.<CreateReward>d__85>(XFramework.UIPanel_Challege.<CreateReward>d__85&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.UIPanel_CompoundDongHua.<setScaleCanCancel>d__36>(XFramework.UIPanel_CompoundDongHua.<setScaleCanCancel>d__36&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.YooResourcesLoader.<InstantiateAsync>d__10>(XFramework.YooResourcesLoader.<InstantiateAsync>d__10&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.YooResourcesLoader.<InstantiateAsync>d__11>(XFramework.YooResourcesLoader.<InstantiateAsync>d__11&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.YooResourcesLoader.<InstantiateAsync>d__12>(XFramework.YooResourcesLoader.<InstantiateAsync>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.YooResourcesLoader.<InstantiateAsync>d__9>(XFramework.YooResourcesLoader.<InstantiateAsync>d__9&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.YooResourcesLoader.<LoadAssetAsync>d__3<object>>(XFramework.YooResourcesLoader.<LoadAssetAsync>d__3<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<XFramework.YooResourcesLoader.<LoadAssetAsync>d__4>(XFramework.YooResourcesLoader.<LoadAssetAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_Logic.HotUpdateMain.<Start>d__6>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_Logic.HotUpdateMain.<Start>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.HybridEventSystem.<OnBossDieEvent>d__5>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.HybridEventSystem.<OnBossDieEvent>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.HybridEventSystem.<OnPlayerDieEvent>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.HybridEventSystem.<OnPlayerDieEvent>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.HybridEventSystem.<SwitchBossScene>d__6>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.HybridEventSystem.<SwitchBossScene>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornSceneHelper.<InitRunTimeScene>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornSceneHelper.<InitRunTimeScene>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<AddReward>d__103>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<AddReward>d__103&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<AddRewardInternal>d__104>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<AddRewardInternal>d__104&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<AddRewardsInternal>d__105>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<AddRewardsInternal>d__105&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<ExitRunTimeScene>d__47>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<ExitRunTimeScene>d__47&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.NetWorkManager.<AttemptReconnect>d__13>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.NetWorkManager.<AttemptReconnect>d__13&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.Global.<CameraShake>d__32>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.Global.<CameraShake>d__32&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.Global.<DoCameraFOV>d__33>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.Global.<DoCameraFOV>d__33&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.IntroGuide.<InitRunTimeScene>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.IntroGuide.<InitRunTimeScene>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.MenuScene.<HandleRedDot>d__31>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.MenuScene.<HandleRedDot>d__31&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.RunTimeScene.<InitRunTimeScene>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.RunTimeScene.<InitRunTimeScene>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UICommonFunButton.<Set3405BankWeb>d__8>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UICommonFunButton.<Set3405BankWeb>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UICommon_Prompt.<AlphaUpdate>d__12>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UICommon_Prompt.<AlphaUpdate>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Activity_Challenge.<DownSelectSet>d__64>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Activity_Challenge.<DownSelectSet>d__64&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Activity_Monopoly.<SetPlayerPos>d__95>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Activity_Monopoly.<SetPlayerPos>d__95&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Activity_NewSign.<CreateOneDaySign>d__41>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Activity_NewSign.<CreateOneDaySign>d__41&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_AnimTools.<Refresh>d__23>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_AnimTools.<Refresh>d__23&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Bank.<Anim>d__36>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Bank.<Anim>d__36&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_BattleTecnology.<SetTechItemUI>d__44>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_BattleTecnology.<SetTechItemUI>d__44&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_BuyEnergy.<Anim>d__40>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_BuyEnergy.<Anim>d__40&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Collection_Unlock.<PlayAnim>d__26>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Collection_Unlock.<PlayAnim>d__26&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Compound.<SpawnItems>d__50>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Compound.<SpawnItems>d__50&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Compound.<SpawnSelectedCompound>d__47>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Compound.<SpawnSelectedCompound>d__47&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_EquipTips.<Guide>d__39>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_EquipTips.<Guide>d__39&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Equipment.<Anim>d__45>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Equipment.<Anim>d__45&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Equipment.<OnClickBag>d__72>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Equipment.<OnClickBag>d__72&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Equipment.<OnClickEquip>d__71>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Equipment.<OnClickEquip>d__71&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_JiyuGame.<Guide>d__39>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_JiyuGame.<Guide>d__39&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_JiyuGame.<OnBtnClickEvent>d__72>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_JiyuGame.<OnBtnClickEvent>d__72&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_JiyuGame.<ShopModuleDelayRefresh>d__52>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_JiyuGame.<ShopModuleDelayRefresh>d__52&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Main.<InitNode>d__89>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Main.<InitNode>d__89&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_MonsterCollection.<PlayAnim>d__36>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_MonsterCollection.<PlayAnim>d__36&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Rebirth.<SetTxtTime>d__20>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Rebirth.<SetTxtTime>d__20&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_RunTimeHUD.<SetEnv>d__85>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_RunTimeHUD.<SetEnv>d__85&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_SelectBoxNomal.<InitNode>d__28>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_SelectBoxNomal.<InitNode>d__28&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Shop.<Module1201_Help_CreateItem>d__114>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Shop.<Module1201_Help_CreateItem>d__114&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Shop.<SetModule1201RedPointState>d__87>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Shop.<SetModule1201RedPointState>d__87&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Shop.<SetModule1302RedPointStateHelp>d__91>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Shop.<SetModule1302RedPointStateHelp>d__91&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Talent.<Anim>d__50>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Talent.<Anim>d__50&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_Guid.<Refresh>d__46>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_Guid.<Refresh>d__46&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_IconBtnItem.<Set3405BankWeb>d__9>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_IconBtnItem.<Set3405BankWeb>d__9&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_Shop_1102_SBox.<InitEffect>d__49>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_Shop_1102_SBox.<InitEffect>d__49&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_Shop_1301_ChapterGift.<ImgSet>d__29>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_Shop_1301_ChapterGift.<ImgSet>d__29&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_Shop_Gift_Item.<InitEffect>d__17>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_Shop_Gift_Item.<InitEffect>d__17&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<Cysharp.Threading.Tasks.AsyncUnit>,XFramework.UIPanel_Equipment.<Anim>d__45>(Cysharp.Threading.Tasks.UniTask.Awaiter<Cysharp.Threading.Tasks.AsyncUnit>&,XFramework.UIPanel_Equipment.<Anim>d__45&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.HybridEventSystem.<OnGuide313>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.HybridEventSystem.<OnGuide313>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornSceneHelper.<InitRunTimeScene>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornSceneHelper.<InitRunTimeScene>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__1>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__1>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__2>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__2>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__3>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__3>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__4>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__4>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__5>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__5>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__7>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__7>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__8>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__8>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__9>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__9>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_1.<<SetRewardOnClick>b__10>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_1.<<SetRewardOnClick>b__10>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__1>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__1>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__2>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__2>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__3>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__3>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__4>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__4>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__5>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__5>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__6>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__6>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__7>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__7>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_1.<<SetRewardOnClickTips>b__8>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_1.<<SetRewardOnClickTips>b__8>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<SetRewardOnClickWithNoBtn>d__100>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<SetRewardOnClickWithNoBtn>d__100&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.IntroGuide.<InitRunTimeScene>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.IntroGuide.<InitRunTimeScene>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.MenuScene.<StandAloneMode>d__20>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.MenuScene.<StandAloneMode>d__20&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.RunTimeScene.<InitRunTimeScene>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.RunTimeScene.<InitRunTimeScene>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UICommon_EquipTips.<RefreshLevelUp>d__32>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UICommon_EquipTips.<RefreshLevelUp>d__32&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Achieve_List.<BottomInit>d__21>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Achieve_List.<BottomInit>d__21&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Achieve_List.<CreateTask>d__18>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Achieve_List.<CreateTask>d__18&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Activity_Challenge.<CreateTasks>d__65>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Activity_Challenge.<CreateTasks>d__65&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Activity_Challenge.<DownSelectSet>d__64>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Activity_Challenge.<DownSelectSet>d__64&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Activity_EnergyShop.<UpdateConatainerItemTopAsync>d__47>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Activity_EnergyShop.<UpdateConatainerItemTopAsync>d__47&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_BattleInfo.<OnClickBindings>d__66>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_BattleInfo.<OnClickBindings>d__66&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_BattleShop.<CreateBindingItem>d__94>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_BattleShop.<CreateBindingItem>d__94&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_BattleShop.<CreateSkillsItem>d__79>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_BattleShop.<CreateSkillsItem>d__79&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_BattleShop.<Guide>d__66>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_BattleShop.<Guide>d__66&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_BattleShop.<OnClickBindings>d__70>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_BattleShop.<OnClickBindings>d__70&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_BattleTecnology.<Guide>d__32>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_BattleTecnology.<Guide>d__32&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_BattleTecnology.<SetTechBtnItem>d__41>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_BattleTecnology.<SetTechBtnItem>d__41&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_BattleTecnology.<SetTechItemUI>d__44>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_BattleTecnology.<SetTechItemUI>d__44&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Challege.<CreateMainTreadAreaInfo>d__80>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Challege.<CreateMainTreadAreaInfo>d__80&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Challege.<SetFromCurrentAreaID>d__75>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Challege.<SetFromCurrentAreaID>d__75&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Compound.<InitPanel>d__39>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Compound.<InitPanel>d__39&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Compound.<OnSelected>d__34>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Compound.<OnSelected>d__34&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Compound.<SpawnItems>d__50>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Compound.<SpawnItems>d__50&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Compound.<SpawnSelectedCompound>d__47>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Compound.<SpawnSelectedCompound>d__47&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_CompoundSuc.<Init>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_CompoundSuc.<Init>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_EquipDownGrade.<InitPanel>d__22>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_EquipDownGrade.<InitPanel>d__22&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_EquipDownGrade.<RefreshLevelUp>d__23>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_EquipDownGrade.<RefreshLevelUp>d__23&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_EquipTips.<Guide>d__39>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_EquipTips.<Guide>d__39&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_EquipTips.<RefreshLevelUp>d__43>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_EquipTips.<RefreshLevelUp>d__43&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Equipment.<InitPanel>d__59>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Equipment.<InitPanel>d__59&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Equipment.<InitTab2WidegetInfo>d__65>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Equipment.<InitTab2WidegetInfo>d__65&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Equipment.<OnClickBag>d__72>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Equipment.<OnClickBag>d__72&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Equipment.<OnClickEquip>d__71>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Equipment.<OnClickEquip>d__71&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_First_Charge.<CreatTime>d__18>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_First_Charge.<CreatTime>d__18&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_JiyuGame.<Guide>d__39>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_JiyuGame.<Guide>d__39&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Login.<GetLocationInfoNew>d__10>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Login.<GetLocationInfoNew>d__10&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Login.<Init>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Login.<Init>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Main.<InitNode>d__89>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Main.<InitNode>d__89&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Main.<OnBuyEnergyBtnClick>d__97>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Main.<OnBuyEnergyBtnClick>d__97&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Main.<UpdateTreasure>d__108>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Main.<UpdateTreasure>d__108&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_MonsterCollection.<CreateBottomBtnList>d__30>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_MonsterCollection.<CreateBottomBtnList>d__30&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Notice.<CreateNoticeItem>d__36>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Notice.<CreateNoticeItem>d__36&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Pass.<CreateItem1>d__70>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Pass.<CreateItem1>d__70&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Pass.<CreateItem>d__80>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Pass.<CreateItem>d__80&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_SelectBoxNomal.<InitNode>d__28>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_SelectBoxNomal.<InitNode>d__28&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Share.<CreateItems>d__33>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Share.<CreateItems>d__33&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Shop.<Module1302_Help_SetGift>d__124>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Shop.<Module1302_Help_SetGift>d__124&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Sweep.<ChangeMagnification>d__67>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Sweep.<ChangeMagnification>d__67&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Talent_Prop.<InitNode>d__12>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Talent_Prop.<InitNode>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Task_DailyAndWeekly.<BottomInit>d__54>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Task_DailyAndWeekly.<BottomInit>d__54&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Task_DailyAndWeekly.<CreateTask>d__47>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Task_DailyAndWeekly.<CreateTask>d__47&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Task_DailyAndWeekly.<TopScoreBoxSet>d__58>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Task_DailyAndWeekly.<TopScoreBoxSet>d__58&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UISubPanel_EnergyShopItem.<OnBtnBuyOnClick>d__23>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UISubPanel_EnergyShopItem.<OnBtnBuyOnClick>d__23&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UISubPanel_EnergyShopItem.<OnSecBtnClick>d__25>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UISubPanel_EnergyShopItem.<OnSecBtnClick>d__25&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UISubPanel_EnergyShopItem.<SetReward>d__21>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UISubPanel_EnergyShopItem.<SetReward>d__21&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UISubPanel_Pass_Token.<CreateItem>d__24>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UISubPanel_Pass_Token.<CreateItem>d__24&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UISubPanel_Shop_1301_ChapterGift.<CreateItem>d__33>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UISubPanel_Shop_1301_ChapterGift.<CreateItem>d__33&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UISubPanel_Shop_Pre.<CreateReward>d__26>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UISubPanel_Shop_Pre.<CreateReward>d__26&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UnityAsyncExtensions.ResourceRequestAwaiter,XFramework.UIPanel_Notice.<SetSpriteByLocalPath>d__35>(Cysharp.Threading.Tasks.UnityAsyncExtensions.ResourceRequestAwaiter&,XFramework.UIPanel_Notice.<SetSpriteByLocalPath>d__35&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.YieldAwaitable.Awaiter,XFramework.UIPanel_Compound.<OnSelected>d__34>(Cysharp.Threading.Tasks.YieldAwaitable.Awaiter&,XFramework.UIPanel_Compound.<OnSelected>d__34&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.YieldAwaitable.Awaiter,XFramework.UIPanel_Equipment.<OnClickBag>d__72>(Cysharp.Threading.Tasks.YieldAwaitable.Awaiter&,XFramework.UIPanel_Equipment.<OnClickBag>d__72&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.YieldAwaitable.Awaiter,XFramework.UIPanel_Equipment.<OnClickEquip>d__71>(Cysharp.Threading.Tasks.YieldAwaitable.Awaiter&,XFramework.UIPanel_Equipment.<OnClickEquip>d__71&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.YieldAwaitable.Awaiter,XFramework.UIPanel_GuideSkillChoose.<ChoseOnSkillGroup>d__21>(Cysharp.Threading.Tasks.YieldAwaitable.Awaiter&,XFramework.UIPanel_GuideSkillChoose.<ChoseOnSkillGroup>d__21&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.YieldAwaitable.Awaiter,XFramework.UIPanel_GuideTips.<Update>d__13>(Cysharp.Threading.Tasks.YieldAwaitable.Awaiter&,XFramework.UIPanel_GuideTips.<Update>d__13&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.YieldAwaitable.Awaiter,XFramework.UIPanel_MonopolyTaskShop.<InitNode>d__43>(Cysharp.Threading.Tasks.YieldAwaitable.Awaiter&,XFramework.UIPanel_MonopolyTaskShop.<InitNode>d__43&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.YieldAwaitable.Awaiter,XFramework.UIPanel_MonsterCollection.<CreateBottomBtnList>d__30>(Cysharp.Threading.Tasks.YieldAwaitable.Awaiter&,XFramework.UIPanel_MonsterCollection.<CreateBottomBtnList>d__30&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.YieldAwaitable.Awaiter,XFramework.UIPanel_SelectBoxNomal.<InitNode>d__28>(Cysharp.Threading.Tasks.YieldAwaitable.Awaiter&,XFramework.UIPanel_SelectBoxNomal.<InitNode>d__28&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_Logic.HotUpdateMain.<Start>d__6>(HotFix_Logic.HotUpdateMain.<Start>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.HybridEventSystem.<OnBossDieEvent>d__5>(HotFix_UI.HybridEventSystem.<OnBossDieEvent>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.HybridEventSystem.<OnGuide313>d__4>(HotFix_UI.HybridEventSystem.<OnGuide313>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.HybridEventSystem.<OnPlayerDieEvent>d__3>(HotFix_UI.HybridEventSystem.<OnPlayerDieEvent>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.HybridEventSystem.<SpawnHybridSpine>d__7>(HotFix_UI.HybridEventSystem.<SpawnHybridSpine>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.HybridEventSystem.<SwitchBossScene>d__6>(HotFix_UI.HybridEventSystem.<SwitchBossScene>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornSceneHelper.<InitRunTimeScene>d__2>(HotFix_UI.UnicornSceneHelper.<InitRunTimeScene>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__0>d>(HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__1>d>(HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__1>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__2>d>(HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__2>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__3>d>(HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__3>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__4>d>(HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__4>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__5>d>(HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__5>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__6>d>(HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__6>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__7>d>(HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__7>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__8>d>(HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__8>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__9>d>(HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_0.<<SetRewardOnClick>b__9>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_1.<<SetRewardOnClick>b__10>d>(HotFix_UI.UnicornUIHelper.<>c__DisplayClass101_1.<<SetRewardOnClick>b__10>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__0>d>(HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__1>d>(HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__1>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__2>d>(HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__2>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__3>d>(HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__3>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__4>d>(HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__4>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__5>d>(HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__5>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__6>d>(HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__6>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__7>d>(HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_0.<<SetRewardOnClickTips>b__7>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_1.<<SetRewardOnClickTips>b__8>d>(HotFix_UI.UnicornUIHelper.<>c__DisplayClass102_1.<<SetRewardOnClickTips>b__8>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<AddReward>d__103>(HotFix_UI.UnicornUIHelper.<AddReward>d__103&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<AddRewardInternal>d__104>(HotFix_UI.UnicornUIHelper.<AddRewardInternal>d__104&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<AddRewardsInternal>d__105>(HotFix_UI.UnicornUIHelper.<AddRewardsInternal>d__105&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<DestroyMonster>d__34>(HotFix_UI.UnicornUIHelper.<DestroyMonster>d__34&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<ExitRunTimeScene>d__47>(HotFix_UI.UnicornUIHelper.<ExitRunTimeScene>d__47&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<SetRewardOnClickWithNoBtn>d__100>(HotFix_UI.UnicornUIHelper.<SetRewardOnClickWithNoBtn>d__100&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<HotFix_UI.NetWorkManager.<AttemptReconnect>d__13>(HotFix_UI.NetWorkManager.<AttemptReconnect>d__13&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.Global.<CameraShake>d__32>(XFramework.Global.<CameraShake>d__32&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.Global.<DoCameraFOV>d__33>(XFramework.Global.<DoCameraFOV>d__33&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.Global.<DoCameraPos>d__34>(XFramework.Global.<DoCameraPos>d__34&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.Global.<DoCameraPosOffset>d__35>(XFramework.Global.<DoCameraPosOffset>d__35&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.IntroGuide.<InitRunTimeScene>d__1>(XFramework.IntroGuide.<InitRunTimeScene>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.MenuScene.<HandleRedDot>d__31>(XFramework.MenuScene.<HandleRedDot>d__31&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.MenuScene.<SendPassTimeMessageDelay>d__16>(XFramework.MenuScene.<SendPassTimeMessageDelay>d__16&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.MenuScene.<SetUpdateDataFromServer>d__4>(XFramework.MenuScene.<SetUpdateDataFromServer>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.MenuScene.<StandAloneMode>d__20>(XFramework.MenuScene.<StandAloneMode>d__20&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.RunTimeScene.<InitRunTimeScene>d__1>(XFramework.RunTimeScene.<InitRunTimeScene>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UICommonFunButton.<Set3405BankWeb>d__8>(XFramework.UICommonFunButton.<Set3405BankWeb>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UICommon_EquipTips.<RefreshLevelUp>d__32>(XFramework.UICommon_EquipTips.<RefreshLevelUp>d__32&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UICommon_Prompt.<AlphaUpdate>d__12>(XFramework.UICommon_Prompt.<AlphaUpdate>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Achieve_List.<BottomInit>d__21>(XFramework.UIPanel_Achieve_List.<BottomInit>d__21&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Achieve_List.<CreateTask>d__18>(XFramework.UIPanel_Achieve_List.<CreateTask>d__18&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Activity_Challenge.<CreateTasks>d__65>(XFramework.UIPanel_Activity_Challenge.<CreateTasks>d__65&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Activity_Challenge.<DownSelectSet>d__64>(XFramework.UIPanel_Activity_Challenge.<DownSelectSet>d__64&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Activity_Challenge.<InitNode>d__52>(XFramework.UIPanel_Activity_Challenge.<InitNode>d__52&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Activity_EnergyShop.<InitShopDes>d__45>(XFramework.UIPanel_Activity_EnergyShop.<InitShopDes>d__45&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Activity_EnergyShop.<UpdateConatainerItemTopAsync>d__47>(XFramework.UIPanel_Activity_EnergyShop.<UpdateConatainerItemTopAsync>d__47&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Activity_Monopoly.<SetPlayerPos>d__95>(XFramework.UIPanel_Activity_Monopoly.<SetPlayerPos>d__95&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Activity_NewSign.<CreateOneDaySign>d__41>(XFramework.UIPanel_Activity_NewSign.<CreateOneDaySign>d__41&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_AnimTools.<Refresh>d__23>(XFramework.UIPanel_AnimTools.<Refresh>d__23&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Bank.<Anim>d__36>(XFramework.UIPanel_Bank.<Anim>d__36&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_BattleInfo.<CreateBindingItem>d__65>(XFramework.UIPanel_BattleInfo.<CreateBindingItem>d__65&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_BattleInfo.<OnClickBindings>d__66>(XFramework.UIPanel_BattleInfo.<OnClickBindings>d__66&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_BattleShop.<CreateBindingItem>d__94>(XFramework.UIPanel_BattleShop.<CreateBindingItem>d__94&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_BattleShop.<CreateSkillsItem>d__79>(XFramework.UIPanel_BattleShop.<CreateSkillsItem>d__79&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_BattleShop.<Guide>d__66>(XFramework.UIPanel_BattleShop.<Guide>d__66&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_BattleShop.<OnClickBindings>d__70>(XFramework.UIPanel_BattleShop.<OnClickBindings>d__70&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_BattleTecnology.<Guide>d__32>(XFramework.UIPanel_BattleTecnology.<Guide>d__32&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_BattleTecnology.<SetTechBtnItem>d__41>(XFramework.UIPanel_BattleTecnology.<SetTechBtnItem>d__41&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_BattleTecnology.<SetTechItemUI>d__44>(XFramework.UIPanel_BattleTecnology.<SetTechItemUI>d__44&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_BuyEnergy.<Anim>d__40>(XFramework.UIPanel_BuyEnergy.<Anim>d__40&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Challege.<CreateMainTreadAreaInfo>d__80>(XFramework.UIPanel_Challege.<CreateMainTreadAreaInfo>d__80&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Challege.<SetFromCurrentAreaID>d__75>(XFramework.UIPanel_Challege.<SetFromCurrentAreaID>d__75&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Collection_Unlock.<PlayAnim>d__26>(XFramework.UIPanel_Collection_Unlock.<PlayAnim>d__26&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Compound.<InitPanel>d__39>(XFramework.UIPanel_Compound.<InitPanel>d__39&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Compound.<OnSelected>d__34>(XFramework.UIPanel_Compound.<OnSelected>d__34&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Compound.<SetSuccess>d__44>(XFramework.UIPanel_Compound.<SetSuccess>d__44&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Compound.<SpawnItems>d__50>(XFramework.UIPanel_Compound.<SpawnItems>d__50&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Compound.<SpawnSelectedCompound>d__47>(XFramework.UIPanel_Compound.<SpawnSelectedCompound>d__47&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_CompoundDongHua.<SetSuccess>d__27>(XFramework.UIPanel_CompoundDongHua.<SetSuccess>d__27&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_CompoundSuc.<Init>d__7>(XFramework.UIPanel_CompoundSuc.<Init>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_EquipDownGrade.<InitPanel>d__22>(XFramework.UIPanel_EquipDownGrade.<InitPanel>d__22&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_EquipDownGrade.<RefreshLevelUp>d__23>(XFramework.UIPanel_EquipDownGrade.<RefreshLevelUp>d__23&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_EquipTips.<Guide>d__39>(XFramework.UIPanel_EquipTips.<Guide>d__39&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_EquipTips.<GuideFinish>d__38>(XFramework.UIPanel_EquipTips.<GuideFinish>d__38&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_EquipTips.<RefreshLevelUp>d__43>(XFramework.UIPanel_EquipTips.<RefreshLevelUp>d__43&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Equipment.<Anim>d__45>(XFramework.UIPanel_Equipment.<Anim>d__45&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Equipment.<InitPanel>d__59>(XFramework.UIPanel_Equipment.<InitPanel>d__59&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Equipment.<InitTab2WidegetInfo>d__65>(XFramework.UIPanel_Equipment.<InitTab2WidegetInfo>d__65&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Equipment.<OnClickBag>d__72>(XFramework.UIPanel_Equipment.<OnClickBag>d__72&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Equipment.<OnClickEquip>d__71>(XFramework.UIPanel_Equipment.<OnClickEquip>d__71&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Equipment.<PlayEquipAnimation>d__54>(XFramework.UIPanel_Equipment.<PlayEquipAnimation>d__54&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_First_Charge.<CreatTime>d__18>(XFramework.UIPanel_First_Charge.<CreatTime>d__18&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_GuideSkillChoose.<ChoseOnSkillGroup>d__21>(XFramework.UIPanel_GuideSkillChoose.<ChoseOnSkillGroup>d__21&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_GuideTips.<Update>d__13>(XFramework.UIPanel_GuideTips.<Update>d__13&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_JiyuGame.<Guide>d__39>(XFramework.UIPanel_JiyuGame.<Guide>d__39&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_JiyuGame.<OnBtnClickEvent>d__72>(XFramework.UIPanel_JiyuGame.<OnBtnClickEvent>d__72&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_JiyuGame.<ShopModuleDelayRefresh>d__52>(XFramework.UIPanel_JiyuGame.<ShopModuleDelayRefresh>d__52&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Login.<GetLocationInfoNew>d__10>(XFramework.UIPanel_Login.<GetLocationInfoNew>d__10&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Login.<Init>d__7>(XFramework.UIPanel_Login.<Init>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Main.<Guide>d__93>(XFramework.UIPanel_Main.<Guide>d__93&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Main.<InitNode>d__89>(XFramework.UIPanel_Main.<InitNode>d__89&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Main.<OnBtnTreasure>d__104>(XFramework.UIPanel_Main.<OnBtnTreasure>d__104&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Main.<OnBuyEnergyBtnClick>d__97>(XFramework.UIPanel_Main.<OnBuyEnergyBtnClick>d__97&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Main.<UpdateTreasure>d__108>(XFramework.UIPanel_Main.<UpdateTreasure>d__108&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_MonopolyShop.<InitNode>d__40>(XFramework.UIPanel_MonopolyShop.<InitNode>d__40&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_MonopolyShop.<Refresh>d__42>(XFramework.UIPanel_MonopolyShop.<Refresh>d__42&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_MonopolyTaskShop.<InitNode>d__43>(XFramework.UIPanel_MonopolyTaskShop.<InitNode>d__43&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_MonopolyTaskShop.<Refresh>d__45>(XFramework.UIPanel_MonopolyTaskShop.<Refresh>d__45&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_MonsterCollection.<CreateBottomBtnList>d__30>(XFramework.UIPanel_MonsterCollection.<CreateBottomBtnList>d__30&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_MonsterCollection.<CreateMainItem>d__31>(XFramework.UIPanel_MonsterCollection.<CreateMainItem>d__31&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_MonsterCollection.<PlayAnim>d__36>(XFramework.UIPanel_MonsterCollection.<PlayAnim>d__36&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Notice.<CreateNoticeItem>d__36>(XFramework.UIPanel_Notice.<CreateNoticeItem>d__36&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Notice.<SetSpriteByLocalPath>d__35>(XFramework.UIPanel_Notice.<SetSpriteByLocalPath>d__35&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Pass.<CreateItem1>d__70>(XFramework.UIPanel_Pass.<CreateItem1>d__70&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Pass.<CreateItem>d__80>(XFramework.UIPanel_Pass.<CreateItem>d__80&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Patrol.<AutoExpTipsFunc>d__73>(XFramework.UIPanel_Patrol.<AutoExpTipsFunc>d__73&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Patrol.<AutoMoneyTipsFuncAsync>d__74>(XFramework.UIPanel_Patrol.<AutoMoneyTipsFuncAsync>d__74&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Rebirth.<SetTxtTime>d__20>(XFramework.UIPanel_Rebirth.<SetTxtTime>d__20&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_RunTimeHUD.<Guide_4>d__84>(XFramework.UIPanel_RunTimeHUD.<Guide_4>d__84&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_RunTimeHUD.<SetBossHp>d__83>(XFramework.UIPanel_RunTimeHUD.<SetBossHp>d__83&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_RunTimeHUD.<SetEnv>d__85>(XFramework.UIPanel_RunTimeHUD.<SetEnv>d__85&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_SelectBoxNomal.<InitNode>d__28>(XFramework.UIPanel_SelectBoxNomal.<InitNode>d__28&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Settings.<Init>d__43>(XFramework.UIPanel_Settings.<Init>d__43&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Share.<CreateItems>d__33>(XFramework.UIPanel_Share.<CreateItems>d__33&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Shop.<Module1201_Help_CreateItem>d__114>(XFramework.UIPanel_Shop.<Module1201_Help_CreateItem>d__114&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Shop.<Module1302_Help_SetGift>d__124>(XFramework.UIPanel_Shop.<Module1302_Help_SetGift>d__124&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Shop.<SetModule1201RedPointState>d__87>(XFramework.UIPanel_Shop.<SetModule1201RedPointState>d__87&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Shop.<SetModule1302RedPointStateHelp>d__91>(XFramework.UIPanel_Shop.<SetModule1302RedPointStateHelp>d__91&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Sweep.<ChangeMagnification>d__67>(XFramework.UIPanel_Sweep.<ChangeMagnification>d__67&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Talent.<Anim>d__50>(XFramework.UIPanel_Talent.<Anim>d__50&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Talent.<SetTalentPropContainer>d__69>(XFramework.UIPanel_Talent.<SetTalentPropContainer>d__69&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Talent_Prop.<InitNode>d__12>(XFramework.UIPanel_Talent_Prop.<InitNode>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Talent_Prop.<ShowPropView>d__15>(XFramework.UIPanel_Talent_Prop.<ShowPropView>d__15&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Talent_Prop.<ShowSkillView>d__14>(XFramework.UIPanel_Talent_Prop.<ShowSkillView>d__14&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Task_DailyAndWeekly.<BottomInit>d__54>(XFramework.UIPanel_Task_DailyAndWeekly.<BottomInit>d__54&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Task_DailyAndWeekly.<CreateTask>d__47>(XFramework.UIPanel_Task_DailyAndWeekly.<CreateTask>d__47&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_Task_DailyAndWeekly.<TopScoreBoxSet>d__58>(XFramework.UIPanel_Task_DailyAndWeekly.<TopScoreBoxSet>d__58&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UIPanel_TechnologyIemBtn.<OnTipBtnClickAsync>d__10>(XFramework.UIPanel_TechnologyIemBtn.<OnTipBtnClickAsync>d__10&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UISubPanel_EnergyShopItem.<OnBtnBuyOnClick>d__23>(XFramework.UISubPanel_EnergyShopItem.<OnBtnBuyOnClick>d__23&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UISubPanel_EnergyShopItem.<OnSecBtnClick>d__25>(XFramework.UISubPanel_EnergyShopItem.<OnSecBtnClick>d__25&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UISubPanel_EnergyShopItem.<SetReward>d__21>(XFramework.UISubPanel_EnergyShopItem.<SetReward>d__21&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UISubPanel_Guid.<InitNode>d__39>(XFramework.UISubPanel_Guid.<InitNode>d__39&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UISubPanel_Guid.<Refresh>d__46>(XFramework.UISubPanel_Guid.<Refresh>d__46&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UISubPanel_IconBtnItem.<Set3405BankWeb>d__9>(XFramework.UISubPanel_IconBtnItem.<Set3405BankWeb>d__9&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UISubPanel_Pass_Token.<CreateItem>d__24>(XFramework.UISubPanel_Pass_Token.<CreateItem>d__24&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UISubPanel_Pass_Token.<ImgInit>d__23>(XFramework.UISubPanel_Pass_Token.<ImgInit>d__23&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_1102_SBox.<InitEffect>d__49>(XFramework.UISubPanel_Shop_1102_SBox.<InitEffect>d__49&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_1301_ChapterGift.<CreateItem>d__33>(XFramework.UISubPanel_Shop_1301_ChapterGift.<CreateItem>d__33&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_1301_ChapterGift.<ImgSet>d__29>(XFramework.UISubPanel_Shop_1301_ChapterGift.<ImgSet>d__29&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_Gift_Item.<InitEffect>d__17>(XFramework.UISubPanel_Shop_Gift_Item.<InitEffect>d__17&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_Pre.<CreateReward>d__26>(XFramework.UISubPanel_Shop_Pre.<CreateReward>d__26&)
		// Cysharp.Threading.Tasks.Internal.StateTuple<Cysharp.Threading.Tasks.UniTask.Awaiter<Cysharp.Threading.Tasks.AsyncUnit>> Cysharp.Threading.Tasks.Internal.StateTuple.Create<Cysharp.Threading.Tasks.UniTask.Awaiter<Cysharp.Threading.Tasks.AsyncUnit>>(Cysharp.Threading.Tasks.UniTask.Awaiter<Cysharp.Threading.Tasks.AsyncUnit>)
		// Cysharp.Threading.Tasks.Internal.StateTuple<Cysharp.Threading.Tasks.UniTask.Awaiter<int>> Cysharp.Threading.Tasks.Internal.StateTuple.Create<Cysharp.Threading.Tasks.UniTask.Awaiter<int>>(Cysharp.Threading.Tasks.UniTask.Awaiter<int>)
		// Cysharp.Threading.Tasks.Internal.StateTuple<Cysharp.Threading.Tasks.UniTask.Awaiter<object>> Cysharp.Threading.Tasks.Internal.StateTuple.Create<Cysharp.Threading.Tasks.UniTask.Awaiter<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>)
		// Cysharp.Threading.Tasks.UniTask<object> Cysharp.Threading.Tasks.UniTaskExtensions.AsUniTask<object>(System.Threading.Tasks.Task<object>,bool)
		// System.Void Cysharp.Threading.Tasks.UniTaskExtensions.Forget<Cysharp.Threading.Tasks.AsyncUnit>(Cysharp.Threading.Tasks.UniTask<Cysharp.Threading.Tasks.AsyncUnit>)
		// System.Void Cysharp.Threading.Tasks.UniTaskExtensions.Forget<int>(Cysharp.Threading.Tasks.UniTask<int>)
		// System.Void Cysharp.Threading.Tasks.UniTaskExtensions.Forget<object>(Cysharp.Threading.Tasks.UniTask<object>)
		// object DG.Tweening.TweenExtensions.Play<object>(object)
		// object DG.Tweening.TweenSettingsExtensions.OnComplete<object>(object,DG.Tweening.TweenCallback)
		// object DG.Tweening.TweenSettingsExtensions.SetAutoKill<object>(object,bool)
		// object DG.Tweening.TweenSettingsExtensions.SetEase<object>(object,DG.Tweening.Ease)
		// object DG.Tweening.TweenSettingsExtensions.SetLoops<object>(object,int,DG.Tweening.LoopType)
		// object DG.Tweening.TweenSettingsExtensions.SetUpdate<object>(object,bool)
		// object Newtonsoft.Json.JsonConvert.DeserializeObject<object>(string)
		// object Newtonsoft.Json.JsonConvert.DeserializeObject<object>(string,Newtonsoft.Json.JsonSerializerSettings)
		// object ProtoBuf.Meta.TypeModel.ActivatorCreate<object>()
		// object ProtoBuf.Meta.TypeModel.CreateInstance<object>(ProtoBuf.ISerializationContext,ProtoBuf.Serializers.ISerializer<object>)
		// ProtoBuf.Serializers.ISerializer<object> ProtoBuf.Meta.TypeModel.GetSerializer<object>()
		// ProtoBuf.Serializers.ISerializer<object> ProtoBuf.Meta.TypeModel.GetSerializer<object>(ProtoBuf.Meta.TypeModel,ProtoBuf.CompatibilityLevel)
		// ProtoBuf.Serializers.ISerializer<object> ProtoBuf.Meta.TypeModel.GetSerializerCore<object>(ProtoBuf.CompatibilityLevel)
		// ProtoBuf.Serializers.ISerializer<object> ProtoBuf.Meta.TypeModel.NoSerializer<object>(ProtoBuf.Meta.TypeModel)
		// long ProtoBuf.Meta.TypeModel.SerializeImpl<object>(ProtoBuf.ProtoWriter.State&,object)
		// ProtoBuf.Serializers.ISerializer<object> ProtoBuf.Meta.TypeModel.TryGetSerializer<object>(ProtoBuf.Meta.TypeModel)
		// object ProtoBuf.ProtoReader.State.<ReadAsRoot>g__ReadFieldOne|102_0<object>(ProtoBuf.ProtoReader.State&,ProtoBuf.Serializers.SerializerFeatures,object,ProtoBuf.Serializers.ISerializer<object>)
		// object ProtoBuf.ProtoReader.State.CreateInstance<object>(ProtoBuf.Serializers.ISerializer<object>)
		// object ProtoBuf.ProtoReader.State.DeserializeRoot<object>(object,ProtoBuf.Serializers.ISerializer<object>)
		// object ProtoBuf.ProtoReader.State.DeserializeRootImpl<object>(object)
		// object ProtoBuf.ProtoReader.State.ReadAny<object>(ProtoBuf.Serializers.SerializerFeatures,object,ProtoBuf.Serializers.ISerializer<object>)
		// object ProtoBuf.ProtoReader.State.ReadAsRoot<object>(object,ProtoBuf.Serializers.ISerializer<object>)
		// object ProtoBuf.ProtoReader.State.ReadMessage<object,object>(ProtoBuf.Serializers.SerializerFeatures,object,object&)
		// object ProtoBuf.ProtoReader.State.ReadMessage<object>(ProtoBuf.Serializers.SerializerFeatures,object,ProtoBuf.Serializers.ISerializer<object>)
		// System.Void ProtoBuf.ProtoWriter.WriteMessage<object>(ProtoBuf.ProtoWriter.State&,object,ProtoBuf.Serializers.ISerializer<object>,ProtoBuf.PrefixStyle,bool)
		// long ProtoBuf.ProtoWriter.State.SerializeRoot<object>(object,ProtoBuf.Serializers.ISerializer<object>)
		// System.Void ProtoBuf.ProtoWriter.State.WriteAsRoot<object>(object,ProtoBuf.Serializers.ISerializer<object>)
		// System.Void ProtoBuf.ProtoWriter.State.WriteMessage<object>(int,ProtoBuf.Serializers.SerializerFeatures,object,ProtoBuf.Serializers.ISerializer<object>)
		// object ProtoBuf.Serializer.Deserialize<object>(System.IO.Stream)
		// System.Void ProtoBuf.Serializer.Serialize<object>(System.IO.Stream,object)
		// System.Void ProtoBuf.Serializer.Serialize<object>(System.IO.Stream,object,object)
		// object System.Activator.CreateInstance<object>()
		// object[] System.Array.Empty<object>()
		// bool System.Linq.Enumerable.Any<System.Collections.Generic.KeyValuePair<int,int>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>,System.Func<System.Collections.Generic.KeyValuePair<int,int>,bool>)
		// int System.Linq.Enumerable.Count<System.Collections.Generic.KeyValuePair<int,int>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>)
		// int System.Linq.Enumerable.Count<System.Collections.Generic.KeyValuePair<long,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,object>>,System.Func<System.Collections.Generic.KeyValuePair<long,object>,bool>)
		// int System.Linq.Enumerable.Count<UnityEngine.Vector3>(System.Collections.Generic.IEnumerable<UnityEngine.Vector3>)
		// int System.Linq.Enumerable.Count<int>(System.Collections.Generic.IEnumerable<int>)
		// int System.Linq.Enumerable.Count<int>(System.Collections.Generic.IEnumerable<int>,System.Func<int,bool>)
		// int System.Linq.Enumerable.Count<object>(System.Collections.Generic.IEnumerable<object>)
		// int System.Linq.Enumerable.Count<ushort>(System.Collections.Generic.IEnumerable<ushort>)
		// System.Collections.Generic.KeyValuePair<int,long> System.Linq.Enumerable.ElementAt<System.Collections.Generic.KeyValuePair<int,long>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,long>>,int)
		// System.Collections.Generic.KeyValuePair<object,int> System.Linq.Enumerable.ElementAt<System.Collections.Generic.KeyValuePair<object,int>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>,int)
		// System.Collections.Generic.KeyValuePair<int,int> System.Linq.Enumerable.First<System.Collections.Generic.KeyValuePair<int,int>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>)
		// int System.Linq.Enumerable.First<int>(System.Collections.Generic.IEnumerable<int>)
		// object System.Linq.Enumerable.First<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.KeyValuePair<int,int> System.Linq.Enumerable.FirstOrDefault<System.Collections.Generic.KeyValuePair<int,int>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>,System.Func<System.Collections.Generic.KeyValuePair<int,int>,bool>)
		// object System.Linq.Enumerable.FirstOrDefault<object>(System.Collections.Generic.IEnumerable<object>)
		// object System.Linq.Enumerable.FirstOrDefault<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// System.Collections.Generic.IEnumerable<System.Linq.IGrouping<int,object>> System.Linq.Enumerable.GroupBy<object,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// float System.Linq.Enumerable.Last<float>(System.Collections.Generic.IEnumerable<float>)
		// int System.Linq.Enumerable.Last<int>(System.Collections.Generic.IEnumerable<int>)
		// int System.Linq.Enumerable.Max<System.Collections.Generic.KeyValuePair<int,int>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>,System.Func<System.Collections.Generic.KeyValuePair<int,int>,int>)
		// int System.Linq.Enumerable.Max<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// System.Linq.IOrderedEnumerable<UnityEngine.Vector3> System.Linq.Enumerable.OrderBy<UnityEngine.Vector3,float>(System.Collections.Generic.IEnumerable<UnityEngine.Vector3>,System.Func<UnityEngine.Vector3,float>)
		// System.Linq.IOrderedEnumerable<int> System.Linq.Enumerable.OrderBy<int,int>(System.Collections.Generic.IEnumerable<int>,System.Func<int,int>)
		// System.Linq.IOrderedEnumerable<object> System.Linq.Enumerable.OrderBy<object,float>(System.Collections.Generic.IEnumerable<object>,System.Func<object,float>)
		// System.Linq.IOrderedEnumerable<object> System.Linq.Enumerable.OrderBy<object,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// System.Linq.IOrderedEnumerable<System.Collections.Generic.KeyValuePair<int,float>> System.Linq.Enumerable.OrderByDescending<System.Collections.Generic.KeyValuePair<int,float>,float>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,float>>,System.Func<System.Collections.Generic.KeyValuePair<int,float>,float>)
		// System.Linq.IOrderedEnumerable<System.Collections.Generic.KeyValuePair<int,int>> System.Linq.Enumerable.OrderByDescending<System.Collections.Generic.KeyValuePair<int,int>,int>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>,System.Func<System.Collections.Generic.KeyValuePair<int,int>,int>)
		// System.Linq.IOrderedEnumerable<System.Collections.Generic.KeyValuePair<object,int>> System.Linq.Enumerable.OrderByDescending<System.Collections.Generic.KeyValuePair<object,int>,int>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>,System.Func<System.Collections.Generic.KeyValuePair<object,int>,int>)
		// System.Linq.IOrderedEnumerable<object> System.Linq.Enumerable.OrderByDescending<object,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Select<System.Collections.Generic.KeyValuePair<int,int>,int>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>,System.Func<System.Collections.Generic.KeyValuePair<int,int>,int>)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Select<object,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Select<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,object>)
		// int System.Linq.Enumerable.Sum<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// System.Linq.IOrderedEnumerable<System.Collections.Generic.KeyValuePair<object,int>> System.Linq.Enumerable.ThenBy<System.Collections.Generic.KeyValuePair<object,int>,int>(System.Linq.IOrderedEnumerable<System.Collections.Generic.KeyValuePair<object,int>>,System.Func<System.Collections.Generic.KeyValuePair<object,int>,int>)
		// int[] System.Linq.Enumerable.ToArray<int>(System.Collections.Generic.IEnumerable<int>)
		// System.Collections.Generic.Dictionary<int,float> System.Linq.Enumerable.ToDictionary<System.Collections.Generic.KeyValuePair<int,float>,int,float>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,float>>,System.Func<System.Collections.Generic.KeyValuePair<int,float>,int>,System.Func<System.Collections.Generic.KeyValuePair<int,float>,float>)
		// System.Collections.Generic.Dictionary<int,float> System.Linq.Enumerable.ToDictionary<System.Collections.Generic.KeyValuePair<int,float>,int,float>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,float>>,System.Func<System.Collections.Generic.KeyValuePair<int,float>,int>,System.Func<System.Collections.Generic.KeyValuePair<int,float>,float>,System.Collections.Generic.IEqualityComparer<int>)
		// System.Collections.Generic.Dictionary<int,int> System.Linq.Enumerable.ToDictionary<System.Collections.Generic.KeyValuePair<int,int>,int,int>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>,System.Func<System.Collections.Generic.KeyValuePair<int,int>,int>,System.Func<System.Collections.Generic.KeyValuePair<int,int>,int>)
		// System.Collections.Generic.Dictionary<int,int> System.Linq.Enumerable.ToDictionary<System.Collections.Generic.KeyValuePair<int,int>,int,int>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>,System.Func<System.Collections.Generic.KeyValuePair<int,int>,int>,System.Func<System.Collections.Generic.KeyValuePair<int,int>,int>,System.Collections.Generic.IEqualityComparer<int>)
		// System.Collections.Generic.Dictionary<object,int> System.Linq.Enumerable.ToDictionary<System.Collections.Generic.KeyValuePair<object,int>,object,int>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>,System.Func<System.Collections.Generic.KeyValuePair<object,int>,object>,System.Func<System.Collections.Generic.KeyValuePair<object,int>,int>)
		// System.Collections.Generic.Dictionary<object,int> System.Linq.Enumerable.ToDictionary<System.Collections.Generic.KeyValuePair<object,int>,object,int>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>,System.Func<System.Collections.Generic.KeyValuePair<object,int>,object>,System.Func<System.Collections.Generic.KeyValuePair<object,int>,int>,System.Collections.Generic.IEqualityComparer<object>)
		// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int,object>> System.Linq.Enumerable.ToList<System.Collections.Generic.KeyValuePair<int,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>)
		// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<long,object>> System.Linq.Enumerable.ToList<System.Collections.Generic.KeyValuePair<long,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,object>>)
		// System.Collections.Generic.List<UnityEngine.Splines.BezierKnot> System.Linq.Enumerable.ToList<UnityEngine.Splines.BezierKnot>(System.Collections.Generic.IEnumerable<UnityEngine.Splines.BezierKnot>)
		// System.Collections.Generic.List<UnityEngine.Vector3> System.Linq.Enumerable.ToList<UnityEngine.Vector3>(System.Collections.Generic.IEnumerable<UnityEngine.Vector3>)
		// System.Collections.Generic.List<float> System.Linq.Enumerable.ToList<float>(System.Collections.Generic.IEnumerable<float>)
		// System.Collections.Generic.List<int> System.Linq.Enumerable.ToList<int>(System.Collections.Generic.IEnumerable<int>)
		// System.Collections.Generic.List<object> System.Linq.Enumerable.ToList<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>> System.Linq.Enumerable.Where<System.Collections.Generic.KeyValuePair<int,int>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>,System.Func<System.Collections.Generic.KeyValuePair<int,int>,bool>)
		// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>> System.Linq.Enumerable.Where<System.Collections.Generic.KeyValuePair<int,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>,System.Func<System.Collections.Generic.KeyValuePair<int,object>,bool>)
		// System.Collections.Generic.IEnumerable<UnityEngine.Vector3> System.Linq.Enumerable.Where<UnityEngine.Vector3>(System.Collections.Generic.IEnumerable<UnityEngine.Vector3>,System.Func<UnityEngine.Vector3,bool>)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Where<int>(System.Collections.Generic.IEnumerable<int>,System.Func<int,bool>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Where<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Iterator<System.Collections.Generic.KeyValuePair<int,int>>.Select<int>(System.Func<System.Collections.Generic.KeyValuePair<int,int>,int>)
		// System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Iterator<object>.Select<int>(System.Func<object,int>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Iterator<object>.Select<object>(System.Func<object,object>)
		// System.Linq.IOrderedEnumerable<System.Collections.Generic.KeyValuePair<object,int>> System.Linq.IOrderedEnumerable<System.Collections.Generic.KeyValuePair<object,int>>.CreateOrderedEnumerable<int>(System.Func<System.Collections.Generic.KeyValuePair<object,int>,int>,System.Collections.Generic.IComparer<int>,bool)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.ErrorMsg.<LogErrorMsg>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.ErrorMsg.<LogErrorMsg>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<AddReward>d__106>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<AddReward>d__106&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,HotFix_UI.UnicornUIHelper.<GoToPanel>d__51>(Cysharp.Threading.Tasks.UniTask.Awaiter&,HotFix_UI.UnicornUIHelper.<GoToPanel>d__51&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.AudioManager.<Init>d__17>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.AudioManager.<Init>d__17&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.DemoEntry.<LoadAsync>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.DemoEntry.<LoadAsync>d__2&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UnicornTweenHelper.<>c__DisplayClass6_0.<<DoScaleTweenOnClickAndLongPress>b__0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UnicornTweenHelper.<>c__DisplayClass6_0.<<DoScaleTweenOnClickAndLongPress>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UnicornTweenHelper.<>c__DisplayClass8_0.<<JiYuOnClick>b__0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UnicornTweenHelper.<>c__DisplayClass8_0.<<JiYuOnClick>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.Global.<SetCameraTarget>d__40>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.Global.<SetCameraTarget>d__40&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.MenuScene.<OnSignResponse>d__47>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.MenuScene.<OnSignResponse>d__47&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.MenuScene.<OnSignSecResponse>d__48>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.MenuScene.<OnSignSecResponse>d__48&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UICommon_Dialog.<InitNode>d__11>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UICommon_Dialog.<InitNode>d__11&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UICommon_Resource.<CreateProperty>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UICommon_Resource.<CreateProperty>d__3&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UICommon_Reward.<>c__DisplayClass10_0.<<Initialize>b__1>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UICommon_Reward.<>c__DisplayClass10_0.<<Initialize>b__1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UICommon_Reward.<>c__DisplayClass10_0.<<Initialize>b__2>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UICommon_Reward.<>c__DisplayClass10_0.<<Initialize>b__2>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UICommon_Reward.<Initialize>d__10>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UICommon_Reward.<Initialize>d__10&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UICommon_Sec_Confirm.<>c__DisplayClass10_0.<<Initialize>b__0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UICommon_Sec_Confirm.<>c__DisplayClass10_0.<<Initialize>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UICommon_Sec_Confirm.<>c__DisplayClass10_0.<<Initialize>b__1>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UICommon_Sec_Confirm.<>c__DisplayClass10_0.<<Initialize>b__1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UICommon_Sec_Confirm.<>c__DisplayClass11_0.<<Initialize>b__0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UICommon_Sec_Confirm.<>c__DisplayClass11_0.<<Initialize>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UICommon_Sec_Confirm.<>c__DisplayClass11_0.<<Initialize>b__1>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UICommon_Sec_Confirm.<>c__DisplayClass11_0.<<Initialize>b__1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIContainerBoxBar.<OnBoxGetResponce>d__35>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIContainerBoxBar.<OnBoxGetResponce>d__35&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIHeadBtn.<>c__DisplayClass7_0.<<Initialize>b__0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIHeadBtn.<>c__DisplayClass7_0.<<Initialize>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIHeadFrameBtn.<>c__DisplayClass6_0.<<Initialize>b__0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIHeadFrameBtn.<>c__DisplayClass6_0.<<Initialize>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Achieve.<Initialize>d__34>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Achieve.<Initialize>d__34&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Achieve.<OnQueryAchievementResponse>d__55>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Achieve.<OnQueryAchievementResponse>d__55&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Achieve_List.<Initialize>d__16>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Achieve_List.<Initialize>d__16&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Activity_Challenge.<Initialize>d__50>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Activity_Challenge.<Initialize>d__50&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Activity_EnergyShop.<Initialize>d__35>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Activity_EnergyShop.<Initialize>d__35&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Activity_Monopoly.<InitNode>d__87>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Activity_Monopoly.<InitNode>d__87&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Activity_Monopoly.<Initialize>d__84>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Activity_Monopoly.<Initialize>d__84&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Activity_Monopoly.<OnRollDiceResponse>d__101>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Activity_Monopoly.<OnRollDiceResponse>d__101&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Activity_NewSign.<InitNode>d__32>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Activity_NewSign.<InitNode>d__32&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Activity_NewSign.<Initialize>d__30>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Activity_NewSign.<Initialize>d__30&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Bank.<Initialize>d__35>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Bank.<Initialize>d__35&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_BattleDamageInfo.<<InitNode>b__10_1>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_BattleDamageInfo.<<InitNode>b__10_1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_BattleDamageInfo.<Initialize>d__9>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_BattleDamageInfo.<Initialize>d__9&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_BattleInfo.<Initialize>d__58>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_BattleInfo.<Initialize>d__58&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_BattleShop.<Initialize>d__64>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_BattleShop.<Initialize>d__64&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_BattleTecnology.<Initialize>d__30>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_BattleTecnology.<Initialize>d__30&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_BuyEnergy.<Initialize>d__39>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_BuyEnergy.<Initialize>d__39&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Compound.<>c__DisplayClass44_0.<<SetSuccess>b__0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Compound.<>c__DisplayClass44_0.<<SetSuccess>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Compound.<OnEquipAllCompoundResponse>d__55>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Compound.<OnEquipAllCompoundResponse>d__55&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_CompoundDongHua.<<SetSuccess>b__27_0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_CompoundDongHua.<<SetSuccess>b__27_0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_CompoundDongHua.<ClosePanel>d__31>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_CompoundDongHua.<ClosePanel>d__31&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_CompoundDongHua.<Initialize>d__26>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_CompoundDongHua.<Initialize>d__26&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_EquipDownGrade.<>c__DisplayClass22_0.<<InitPanel>b__1>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_EquipDownGrade.<>c__DisplayClass22_0.<<InitPanel>b__1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_EquipDownGrade.<Initialize>d__19>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_EquipDownGrade.<Initialize>d__19&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Fail.<Initialize>d__18>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Fail.<Initialize>d__18&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_First_Charge.<Initialize>d__15>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_First_Charge.<Initialize>d__15&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_GuideTips.<InitNode>d__12>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_GuideTips.<InitNode>d__12&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Login.<>c__DisplayClass9_0.<<OnLoginResponse>b__0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Login.<>c__DisplayClass9_0.<<OnLoginResponse>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Mail.<<Initialize>b__38_0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Mail.<<Initialize>b__38_0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Mail.<<Initialize>b__38_1>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Mail.<<Initialize>b__38_1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Mail.<Initialize>d__38>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Mail.<Initialize>d__38&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Main.<>c__DisplayClass89_0.<<InitNode>b__0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Main.<>c__DisplayClass89_0.<<InitNode>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Main.<>c__DisplayClass89_11.<<InitNode>b__36>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Main.<>c__DisplayClass89_11.<<InitNode>b__36>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Main.<>c__DisplayClass89_8.<<InitNode>b__29>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Main.<>c__DisplayClass89_8.<<InitNode>b__29>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Main.<>c__DisplayClass89_8.<<InitNode>b__31>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Main.<>c__DisplayClass89_8.<<InitNode>b__31>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Main.<>c__DisplayClass89_9.<<InitNode>b__32>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Main.<>c__DisplayClass89_9.<<InitNode>b__32>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Main.<OnGuideIdFinished>d__91>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Main.<OnGuideIdFinished>d__91&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Main.<OnStartButtonClick>d__129>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Main.<OnStartButtonClick>d__129&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_MonopolyShop.<Initialize>d__38>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_MonopolyShop.<Initialize>d__38&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_MonopolyTaskShop.<Initialize>d__41>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_MonopolyTaskShop.<Initialize>d__41&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_MonsterCollection.<>c__DisplayClass37_2.<<ProvideData>b__5>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_MonsterCollection.<>c__DisplayClass37_2.<<ProvideData>b__5>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_MonsterCollection.<>c__DisplayClass37_5.<<ProvideData>b__15>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_MonsterCollection.<>c__DisplayClass37_5.<<ProvideData>b__15>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Notice.<Initialize>d__27>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Notice.<Initialize>d__27&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_ParasTest.<<Initialize>b__24_5>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_ParasTest.<<Initialize>b__24_5>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_ParasTest.<Initialize>d__24>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_ParasTest.<Initialize>d__24&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Pass.<Initialize>d__51>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Pass.<Initialize>d__51&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Patrol.<<InitWidgetAction>b__71_0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Patrol.<<InitWidgetAction>b__71_0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Patrol.<Initialize>d__68>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Patrol.<Initialize>d__68&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Person.<HeadImgInit>d__63>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Person.<HeadImgInit>d__63&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Quest.<Initialize>d__10>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Quest.<Initialize>d__10&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Rebirth.<>c__DisplayClass19_0.<<InitNode>b__0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Rebirth.<>c__DisplayClass19_0.<<InitNode>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Rebirth.<>c__DisplayClass19_0.<<InitNode>b__1>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Rebirth.<>c__DisplayClass19_0.<<InitNode>b__1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Rebirth.<>c__DisplayClass19_0.<<InitNode>b__2>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Rebirth.<>c__DisplayClass19_0.<<InitNode>b__2>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Rebirth.<InitNode>d__19>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Rebirth.<InitNode>d__19&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Rebirth.<Initialize>d__17>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Rebirth.<Initialize>d__17&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_ReturnConfirm.<<InitNode>b__10_0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_ReturnConfirm.<<InitNode>b__10_0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_ReturnConfirm.<<InitNode>b__10_1>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_ReturnConfirm.<<InitNode>b__10_1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_ReturnConfirm.<Initialize>d__8>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_ReturnConfirm.<Initialize>d__8&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_RunTimeHUD.<InitGuidScene>d__73>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_RunTimeHUD.<InitGuidScene>d__73&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_RunTimeHUD.<OnGuideOrderFinished>d__75>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_RunTimeHUD.<OnGuideOrderFinished>d__75&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Settings.<>c.<<Init>b__43_8>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Settings.<>c.<<Init>b__43_8>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Settings.<Initialize>d__41>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Settings.<Initialize>d__41&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Share.<Initialize>d__25>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Share.<Initialize>d__25&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Shop.<Module1101>d__102>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Shop.<Module1101>d__102&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Shop.<Module1404>d__141>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Shop.<Module1404>d__141&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Sign.<Initialize>d__34>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Sign.<Initialize>d__34&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Sign.<SetState>d__43>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Sign.<SetState>d__43&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Success.<Initialize>d__15>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Success.<Initialize>d__15&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Sweep.<<InitBtn>b__65_0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Sweep.<<InitBtn>b__65_0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Sweep.<Initialize>d__59>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Sweep.<Initialize>d__59&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Sweep.<UpdateCardState>d__62>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Sweep.<UpdateCardState>d__62&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Talent.<ActPlyarAnimation>d__55>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Talent.<ActPlyarAnimation>d__55&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Talent_Prop.<Initialize>d__11>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Talent_Prop.<Initialize>d__11&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Task_DailyAndWeekly.<Initialize>d__38>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Task_DailyAndWeekly.<Initialize>d__38&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_Task_DailyAndWeekly.<OnCliamTaskResponse>d__49>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_Task_DailyAndWeekly.<OnCliamTaskResponse>d__49&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPanel_UnlockBlock.<Initialize>d__19>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPanel_UnlockBlock.<Initialize>d__19&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPlayerInformtion.<>c__DisplayClass41_0.<<Initialize>b__3>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPlayerInformtion.<>c__DisplayClass41_0.<<Initialize>b__3>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPlayerInformtion.<>c__DisplayClass41_0.<<Initialize>b__4>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPlayerInformtion.<>c__DisplayClass41_0.<<Initialize>b__4>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPlayerInformtion.<>c__DisplayClass41_0.<<Initialize>b__5>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPlayerInformtion.<>c__DisplayClass41_0.<<Initialize>b__5>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPlayerInformtion.<InitFrameItem>d__45>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPlayerInformtion.<InitFrameItem>d__45&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPlayerInformtion.<InitHeadItem>d__44>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPlayerInformtion.<InitHeadItem>d__44&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIPlayerInformtion.<Initialize>d__41>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIPlayerInformtion.<Initialize>d__41&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIResource.<>c__DisplayClass2_0.<<ResourceAni>b__2>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIResource.<>c__DisplayClass2_0.<<ResourceAni>b__2>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIResource.<>c__DisplayClass2_1.<<ResourceAni>b__0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIResource.<>c__DisplayClass2_1.<<ResourceAni>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UIResource.<>c__DisplayClass2_1.<<ResourceAni>b__1>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UIResource.<>c__DisplayClass2_1.<<ResourceAni>b__1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISettlementItem.<Initialize>d__5>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISettlementItem.<Initialize>d__5&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_Pass_Token.<Initialize>d__18>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_Pass_Token.<Initialize>d__18&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_Person_ChangeName.<Initialize>d__32>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_Person_ChangeName.<Initialize>d__32&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_Person_ChangeName.<SelectStatus>d__38>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_Person_ChangeName.<SelectStatus>d__38&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_Person_ChangeNameCenter.<Initialize>d__31>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_Person_ChangeNameCenter.<Initialize>d__31&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_Person_ChangeNameCenter.<SelectStatus>d__37>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_Person_ChangeNameCenter.<SelectStatus>d__37&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_Person_UserInfo.<HeadImageSet>d__31>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_Person_UserInfo.<HeadImageSet>d__31&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_Person_UserInfo.<HeadImgInit>d__38>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_Person_UserInfo.<HeadImgInit>d__38&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_Person_UserInfo.<HeadSet>d__39>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_Person_UserInfo.<HeadSet>d__39&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_Shop_1102_SBox.<BtnTxtInit>d__59>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_Shop_1102_SBox.<BtnTxtInit>d__59&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_Shop_1103_Box.<TxtInit>d__42>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_Shop_1103_Box.<TxtInit>d__42&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_Shop_1403_Fund.<Module1403_Help_CreateItem>d__52>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_Shop_1403_Fund.<Module1403_Help_CreateItem>d__52&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_Shop_Draw.<DisplayInit>d__46>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_Shop_Draw.<DisplayInit>d__46&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_Shop_Fund_List.<Initialize>d__24>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_Shop_Fund_List.<Initialize>d__24&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_Shop_Pre.<InScreenOrNot>d__29>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_Shop_Pre.<InScreenOrNot>d__29&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,XFramework.UISubPanel_Shop_Pre.<Initialize>d__22>(Cysharp.Threading.Tasks.UniTask.Awaiter&,XFramework.UISubPanel_Shop_Pre.<Initialize>d__22&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<Cysharp.Threading.Tasks.AsyncUnit>,XFramework.UIPanel_Activity_NewSign.<InitNode>d__32>(Cysharp.Threading.Tasks.UniTask.Awaiter<Cysharp.Threading.Tasks.AsyncUnit>&,XFramework.UIPanel_Activity_NewSign.<InitNode>d__32&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<Cysharp.Threading.Tasks.AsyncUnit>,XFramework.UIPanel_Compound.<>c__DisplayClass39_0.<<InitPanel>b__3>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<Cysharp.Threading.Tasks.AsyncUnit>&,XFramework.UIPanel_Compound.<>c__DisplayClass39_0.<<InitPanel>b__3>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<Cysharp.Threading.Tasks.AsyncUnit>,XFramework.UISubPanel_Shop_Draw.<DisplayInit>d__46>(Cysharp.Threading.Tasks.UniTask.Awaiter<Cysharp.Threading.Tasks.AsyncUnit>&,XFramework.UISubPanel_Shop_Draw.<DisplayInit>d__46&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<float>,XFramework.UIContainerBoxBar.<Initialize>d__29>(Cysharp.Threading.Tasks.UniTask.Awaiter<float>&,XFramework.UIContainerBoxBar.<Initialize>d__29&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<int>,XFramework.UIPanel_Challege.<OnMainChallengeAreaClick>d__81>(Cysharp.Threading.Tasks.UniTask.Awaiter<int>&,XFramework.UIPanel_Challege.<OnMainChallengeAreaClick>d__81&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<>c__DisplayClass76_0.<<SetEquipOnClick>b__0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<>c__DisplayClass76_0.<<SetEquipOnClick>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<>c__DisplayClass77_0.<<SetEquipOnClick>b__0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<>c__DisplayClass77_0.<<SetEquipOnClick>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<CreateTagPools>d__155>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<CreateTagPools>d__155&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<GoToPanel>d__51>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<GoToPanel>d__51&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<QuickSucceed>d__27>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<QuickSucceed>d__27&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.UnicornUIHelper.<ReConnect>d__26>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.UnicornUIHelper.<ReConnect>d__26&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,HotFix_UI.NetWorkManager.<Init>d__15>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,HotFix_UI.NetWorkManager.<Init>d__15&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.MenuScene.<>c.<<OnCompleted>b__2_1>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.MenuScene.<>c.<<OnCompleted>b__2_1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.MenuScene.<OnBoardCastPaymentResponse>d__33>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.MenuScene.<OnBoardCastPaymentResponse>d__33&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.MenuScene.<OnUpdate>d__51>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.MenuScene.<OnUpdate>d__51&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIBagItem.<>c__DisplayClass10_0.<<Initialize>b__0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIBagItem.<>c__DisplayClass10_0.<<Initialize>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UICommon_Btn1.<OnClickBtn>d__12>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UICommon_Btn1.<OnClickBtn>d__12&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UICommon_EquipTips.<Initialize>d__30>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UICommon_EquipTips.<Initialize>d__30&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UICommon_Resource.<CreateProperty>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UICommon_Resource.<CreateProperty>d__3&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UICommon_Sec_Confirm.<>c__DisplayClass10_0.<<Initialize>b__2>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UICommon_Sec_Confirm.<>c__DisplayClass10_0.<<Initialize>b__2>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UICommon_Sec_Confirm.<>c__DisplayClass11_0.<<Initialize>b__2>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UICommon_Sec_Confirm.<>c__DisplayClass11_0.<<Initialize>b__2>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIContainer_Bar.<Initialize>d__11>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIContainer_Bar.<Initialize>d__11&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Achieve.<>c__DisplayClass36_0.<<InitNode>b__2>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Achieve.<>c__DisplayClass36_0.<<InitNode>b__2>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Achieve.<>c__DisplayClass46_0.<<CreateGroup>b__2>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Achieve.<>c__DisplayClass46_0.<<CreateGroup>b__2>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Activity_Monopoly.<>c__DisplayClass101_1.<<OnRollDiceResponse>b__0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Activity_Monopoly.<>c__DisplayClass101_1.<<OnRollDiceResponse>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Activity_Monopoly.<>c__DisplayClass87_0.<<InitNode>b__5>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Activity_Monopoly.<>c__DisplayClass87_0.<<InitNode>b__5>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Activity_Monopoly.<>c__DisplayClass87_0.<<InitNode>b__7>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Activity_Monopoly.<>c__DisplayClass87_0.<<InitNode>b__7>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Activity_Monopoly.<InitGridList>d__90>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Activity_Monopoly.<InitGridList>d__90&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Activity_Monopoly.<OnRollDiceResponse>d__101>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Activity_Monopoly.<OnRollDiceResponse>d__101&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_BattleDamageInfo.<InitNode>d__10>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_BattleDamageInfo.<InitNode>d__10&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_BattleShop.<>c__DisplayClass67_0.<<InitPanel>b__0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_BattleShop.<>c__DisplayClass67_0.<<InitPanel>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_BuyDice.<>c__DisplayClass26_0.<<InitNode>b__5>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_BuyDice.<>c__DisplayClass26_0.<<InitNode>b__5>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_BuyDice.<InitNode>d__26>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_BuyDice.<InitNode>d__26&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_BuyDice.<OnBuyDiceResponse>d__27>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_BuyDice.<OnBuyDiceResponse>d__27&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Challege.<CreateAreaTreadAreaInfo>d__71>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Challege.<CreateAreaTreadAreaInfo>d__71&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Challege.<CreateEventDes>d__84>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Challege.<CreateEventDes>d__84&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Compound.<>c__DisplayClass39_0.<<InitPanel>b__3>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Compound.<>c__DisplayClass39_0.<<InitPanel>b__3>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Compound.<>c__DisplayClass39_0.<<InitPanel>b__5>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Compound.<>c__DisplayClass39_0.<<InitPanel>b__5>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_EquipTips.<Initialize>d__37>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_EquipTips.<Initialize>d__37&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Fail.<InitReWardItem>d__21>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Fail.<InitReWardItem>d__21&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Mail.<InitTopContent>d__45>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Mail.<InitTopContent>d__45&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Main.<OnChangeNameStatusResponse>d__94>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Main.<OnChangeNameStatusResponse>d__94&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Main.<OnClickTagFunc>d__111>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Main.<OnClickTagFunc>d__111&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Main.<OnStartButtonClick>d__129>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Main.<OnStartButtonClick>d__129&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Main.<OpenPlayerInfo>d__119>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Main.<OpenPlayerInfo>d__119&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_MonopolyShop.<OnExchangeResponse>d__41>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_MonopolyShop.<OnExchangeResponse>d__41&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_MonopolyShop.<ProvideData>d__47>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_MonopolyShop.<ProvideData>d__47&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_MonopolyTaskShop.<OnGetTaskResponse>d__44>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_MonopolyTaskShop.<OnGetTaskResponse>d__44&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_MonsterCollection.<>c__DisplayClass37_2.<<ProvideData>b__5>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_MonsterCollection.<>c__DisplayClass37_2.<<ProvideData>b__5>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_MonsterCollection.<>c__DisplayClass37_5.<<ProvideData>b__15>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_MonsterCollection.<>c__DisplayClass37_5.<<ProvideData>b__15>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_MonsterCollection.<ProvideData>d__37>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_MonsterCollection.<ProvideData>d__37&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Pass.<<BtnInit>b__61_1>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Pass.<<BtnInit>b__61_1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Pass.<<CreateItem>b__80_4>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Pass.<<CreateItem>b__80_4>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Pass.<>c__DisplayClass74_0.<<BoxSet>b__0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Pass.<>c__DisplayClass74_0.<<BoxSet>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Patrol.<CreateAutoAward>d__91>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Patrol.<CreateAutoAward>d__91&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Patrol.<CreateRapidAward>d__89>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Patrol.<CreateRapidAward>d__89&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Person.<Btn52OnClick>d__61>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Person.<Btn52OnClick>d__61&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Person.<CreatePrompt>d__53>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Person.<CreatePrompt>d__53&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Person.<OnChangeNameStatusResponse>d__52>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Person.<OnChangeNameStatusResponse>d__52&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_RunTimeHUD.<Initialize>d__69>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_RunTimeHUD.<Initialize>d__69&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_RunTimeHUD.<Initialize>d__80>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_RunTimeHUD.<Initialize>d__80&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_SelectBoxNomal.<OnSelfChooseBoxResponse>d__29>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_SelectBoxNomal.<OnSelfChooseBoxResponse>d__29&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Sign.<OnBigResponse>d__49>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Sign.<OnBigResponse>d__49&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Sign.<OnDailResponse>d__46>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Sign.<OnDailResponse>d__46&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Sign.<SetState>d__43>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Sign.<SetState>d__43&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Success.<InitReWardItem>d__19>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Success.<InitReWardItem>d__19&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Talent.<SetTanlentPropDetails>d__78>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Talent.<SetTanlentPropDetails>d__78&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPanel_Task_DailyAndWeekly.<OnGetAllDailyResponse>d__45>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPanel_Task_DailyAndWeekly.<OnGetAllDailyResponse>d__45&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPlayerInformtion.<InitFrameItem>d__45>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPlayerInformtion.<InitFrameItem>d__45&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIPlayerInformtion.<InitHeadItem>d__44>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIPlayerInformtion.<InitHeadItem>d__44&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UIResource.<ResourceAni>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UIResource.<ResourceAni>d__2&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UISubPanel_Equipment.<OnBtnClickEvent>d__18>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UISubPanel_Equipment.<OnBtnClickEvent>d__18&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UISubPanel_Person_ChangeName.<CreatePrompt>d__41>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UISubPanel_Person_ChangeName.<CreatePrompt>d__41&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UISubPanel_Person_ChangeNameCenter.<CreatePrompt>d__40>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UISubPanel_Person_ChangeNameCenter.<CreatePrompt>d__40&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UISubPanel_Person_UserInfo.<BottomInit>d__36>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UISubPanel_Person_UserInfo.<BottomInit>d__36&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UISubPanel_Person_UserInfo.<OnChangeNameStatusResponse>d__45>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UISubPanel_Person_UserInfo.<OnChangeNameStatusResponse>d__45&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UISubPanel_Shop_Draw.<DisplayInit>d__46>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UISubPanel_Shop_Draw.<DisplayInit>d__46&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UISubPanel_Shop_Pre.<BottomInit>d__24>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UISubPanel_Shop_Pre.<BottomInit>d__24&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UISubPanel_Shop_Pre.<CreateTipHelp>d__28>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UISubPanel_Shop_Pre.<CreateTipHelp>d__28&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,XFramework.UISubPanel_Shop_item.<>c__DisplayClass51_0.<<Initialize>b__5>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,XFramework.UISubPanel_Shop_item.<>c__DisplayClass51_0.<<Initialize>b__5>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.YieldAwaitable.Awaiter,HotFix_UI.UnicornUIHelper.<SetForceGuideRectUI>d__56>(Cysharp.Threading.Tasks.YieldAwaitable.Awaiter&,HotFix_UI.UnicornUIHelper.<SetForceGuideRectUI>d__56&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.YieldAwaitable.Awaiter,XFramework.UICommon_ItemTips.<Initialize>d__13>(Cysharp.Threading.Tasks.YieldAwaitable.Awaiter&,XFramework.UICommon_ItemTips.<Initialize>d__13&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.YieldAwaitable.Awaiter,XFramework.UIPanel_Activity_Monopoly.<InitNode>d__87>(Cysharp.Threading.Tasks.YieldAwaitable.Awaiter&,XFramework.UIPanel_Activity_Monopoly.<InitNode>d__87&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.YieldAwaitable.Awaiter,XFramework.UIPanel_Main.<>c__DisplayClass89_11.<<InitNode>b__36>d>(Cysharp.Threading.Tasks.YieldAwaitable.Awaiter&,XFramework.UIPanel_Main.<>c__DisplayClass89_11.<<InitNode>b__36>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.YieldAwaitable.Awaiter,XFramework.UIPanel_Main.<>c__DisplayClass89_9.<<InitNode>b__32>d>(Cysharp.Threading.Tasks.YieldAwaitable.Awaiter&,XFramework.UIPanel_Main.<>c__DisplayClass89_9.<<InitNode>b__32>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.YieldAwaitable.Awaiter,XFramework.UISubPanel_Shop_1403_Fund.<Module1403_Help_CreateItem>d__52>(Cysharp.Threading.Tasks.YieldAwaitable.Awaiter&,XFramework.UISubPanel_Shop_1403_Fund.<Module1403_Help_CreateItem>d__52&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.YieldAwaitable.Awaiter,XFramework.UISubPanel_Shop_Pre.<CreateTipHelp>d__28>(Cysharp.Threading.Tasks.YieldAwaitable.Awaiter&,XFramework.UISubPanel_Shop_Pre.<CreateTipHelp>d__28&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,XFramework.UISubPanel_MailItem.<OnReceiveGift>d__20>(System.Runtime.CompilerServices.TaskAwaiter&,XFramework.UISubPanel_MailItem.<OnReceiveGift>d__20&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<EquipItemBtnTest.<<Start>b__39_0>d>(EquipItemBtnTest.<<Start>b__39_0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<HotFix_UI.ErrorMsg.<LogErrorMsg>d__0>(HotFix_UI.ErrorMsg.<LogErrorMsg>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<>c__DisplayClass76_0.<<SetEquipOnClick>b__0>d>(HotFix_UI.UnicornUIHelper.<>c__DisplayClass76_0.<<SetEquipOnClick>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<>c__DisplayClass77_0.<<SetEquipOnClick>b__0>d>(HotFix_UI.UnicornUIHelper.<>c__DisplayClass77_0.<<SetEquipOnClick>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<AddReward>d__106>(HotFix_UI.UnicornUIHelper.<AddReward>d__106&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<CreateTagPools>d__155>(HotFix_UI.UnicornUIHelper.<CreateTagPools>d__155&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<GoToPanel>d__51>(HotFix_UI.UnicornUIHelper.<GoToPanel>d__51&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<QuickSucceed>d__27>(HotFix_UI.UnicornUIHelper.<QuickSucceed>d__27&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<ReConnect>d__26>(HotFix_UI.UnicornUIHelper.<ReConnect>d__26&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<HotFix_UI.UnicornUIHelper.<SetForceGuideRectUI>d__56>(HotFix_UI.UnicornUIHelper.<SetForceGuideRectUI>d__56&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<HotFix_UI.NetWorkManager.<Init>d__15>(HotFix_UI.NetWorkManager.<Init>d__15&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.AudioManager.<Init>d__17>(XFramework.AudioManager.<Init>d__17&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.DemoEntry.<LoadAsync>d__2>(XFramework.DemoEntry.<LoadAsync>d__2&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UnicornTweenHelper.<>c__DisplayClass6_0.<<DoScaleTweenOnClickAndLongPress>b__0>d>(XFramework.UnicornTweenHelper.<>c__DisplayClass6_0.<<DoScaleTweenOnClickAndLongPress>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UnicornTweenHelper.<>c__DisplayClass8_0.<<JiYuOnClick>b__0>d>(XFramework.UnicornTweenHelper.<>c__DisplayClass8_0.<<JiYuOnClick>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.Global.<SetCameraTarget>d__40>(XFramework.Global.<SetCameraTarget>d__40&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.Login.<OnCompleted>d__1>(XFramework.Login.<OnCompleted>d__1&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.MenuScene.<>c.<<OnCompleted>b__2_1>d>(XFramework.MenuScene.<>c.<<OnCompleted>b__2_1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.MenuScene.<GetObjects>d__1>(XFramework.MenuScene.<GetObjects>d__1&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.MenuScene.<OnBoardCastPaymentResponse>d__33>(XFramework.MenuScene.<OnBoardCastPaymentResponse>d__33&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.MenuScene.<OnBoardCastUpdateFuncTaskResponse>d__34>(XFramework.MenuScene.<OnBoardCastUpdateFuncTaskResponse>d__34&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.MenuScene.<OnCompleted>d__2>(XFramework.MenuScene.<OnCompleted>d__2&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.MenuScene.<OnOpenMainPanelResponse>d__40>(XFramework.MenuScene.<OnOpenMainPanelResponse>d__40&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.MenuScene.<OnQueryMonopolyTaskResponse>d__7>(XFramework.MenuScene.<OnQueryMonopolyTaskResponse>d__7&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.MenuScene.<OnQuerySettingsResponse>d__50>(XFramework.MenuScene.<OnQuerySettingsResponse>d__50&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.MenuScene.<OnSignResponse>d__47>(XFramework.MenuScene.<OnSignResponse>d__47&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.MenuScene.<OnSignSecResponse>d__48>(XFramework.MenuScene.<OnSignSecResponse>d__48&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.MenuScene.<OnUpdate>d__51>(XFramework.MenuScene.<OnUpdate>d__51&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.RedDotManager.<ChangeRedPointCnt>d__17>(XFramework.RedDotManager.<ChangeRedPointCnt>d__17&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIBagItem.<>c__DisplayClass10_0.<<Initialize>b__0>d>(XFramework.UIBagItem.<>c__DisplayClass10_0.<<Initialize>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UICommon_Btn1.<OnClickBtn>d__12>(XFramework.UICommon_Btn1.<OnClickBtn>d__12&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UICommon_Dialog.<InitNode>d__11>(XFramework.UICommon_Dialog.<InitNode>d__11&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UICommon_EquipTips.<Initialize>d__30>(XFramework.UICommon_EquipTips.<Initialize>d__30&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UICommon_ItemTips.<Initialize>d__13>(XFramework.UICommon_ItemTips.<Initialize>d__13&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UICommon_Prompt.<>c__DisplayClass11_0.<<StartAnimation>b__0>d>(XFramework.UICommon_Prompt.<>c__DisplayClass11_0.<<StartAnimation>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UICommon_Resource.<CreateProperty>d__3>(XFramework.UICommon_Resource.<CreateProperty>d__3&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UICommon_Reward.<>c__DisplayClass10_0.<<Initialize>b__1>d>(XFramework.UICommon_Reward.<>c__DisplayClass10_0.<<Initialize>b__1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UICommon_Reward.<>c__DisplayClass10_0.<<Initialize>b__2>d>(XFramework.UICommon_Reward.<>c__DisplayClass10_0.<<Initialize>b__2>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UICommon_Reward.<Initialize>d__10>(XFramework.UICommon_Reward.<Initialize>d__10&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UICommon_Reward_Tip.<Initialize>d__7>(XFramework.UICommon_Reward_Tip.<Initialize>d__7&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UICommon_Sec_Confirm.<>c__DisplayClass10_0.<<Initialize>b__0>d>(XFramework.UICommon_Sec_Confirm.<>c__DisplayClass10_0.<<Initialize>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UICommon_Sec_Confirm.<>c__DisplayClass10_0.<<Initialize>b__1>d>(XFramework.UICommon_Sec_Confirm.<>c__DisplayClass10_0.<<Initialize>b__1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UICommon_Sec_Confirm.<>c__DisplayClass10_0.<<Initialize>b__2>d>(XFramework.UICommon_Sec_Confirm.<>c__DisplayClass10_0.<<Initialize>b__2>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UICommon_Sec_Confirm.<>c__DisplayClass11_0.<<Initialize>b__0>d>(XFramework.UICommon_Sec_Confirm.<>c__DisplayClass11_0.<<Initialize>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UICommon_Sec_Confirm.<>c__DisplayClass11_0.<<Initialize>b__1>d>(XFramework.UICommon_Sec_Confirm.<>c__DisplayClass11_0.<<Initialize>b__1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UICommon_Sec_Confirm.<>c__DisplayClass11_0.<<Initialize>b__2>d>(XFramework.UICommon_Sec_Confirm.<>c__DisplayClass11_0.<<Initialize>b__2>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIContainerBoxBar.<Initialize>d__29>(XFramework.UIContainerBoxBar.<Initialize>d__29&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIContainerBoxBar.<OnBoxGetResponce>d__35>(XFramework.UIContainerBoxBar.<OnBoxGetResponce>d__35&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIContainer_Bar.<Initialize>d__11>(XFramework.UIContainer_Bar.<Initialize>d__11&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIHeadBtn.<>c__DisplayClass7_0.<<Initialize>b__0>d>(XFramework.UIHeadBtn.<>c__DisplayClass7_0.<<Initialize>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIHeadFrameBtn.<>c__DisplayClass6_0.<<Initialize>b__0>d>(XFramework.UIHeadFrameBtn.<>c__DisplayClass6_0.<<Initialize>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UILevelBox.<onGetButtonClicked>d__12>(XFramework.UILevelBox.<onGetButtonClicked>d__12&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Achieve.<>c__DisplayClass36_0.<<InitNode>b__1>d>(XFramework.UIPanel_Achieve.<>c__DisplayClass36_0.<<InitNode>b__1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Achieve.<>c__DisplayClass36_0.<<InitNode>b__2>d>(XFramework.UIPanel_Achieve.<>c__DisplayClass36_0.<<InitNode>b__2>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Achieve.<>c__DisplayClass46_0.<<CreateGroup>b__2>d>(XFramework.UIPanel_Achieve.<>c__DisplayClass46_0.<<CreateGroup>b__2>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Achieve.<Initialize>d__34>(XFramework.UIPanel_Achieve.<Initialize>d__34&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Achieve.<OnQueryAchievementResponse>d__55>(XFramework.UIPanel_Achieve.<OnQueryAchievementResponse>d__55&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Achieve.<PosDownSet>d__50>(XFramework.UIPanel_Achieve.<PosDownSet>d__50&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Achieve_List.<Initialize>d__16>(XFramework.UIPanel_Achieve_List.<Initialize>d__16&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Activity_Challenge.<Initialize>d__50>(XFramework.UIPanel_Activity_Challenge.<Initialize>d__50&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Activity_EnergyShop.<InitEffect>d__36>(XFramework.UIPanel_Activity_EnergyShop.<InitEffect>d__36&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Activity_EnergyShop.<Initialize>d__35>(XFramework.UIPanel_Activity_EnergyShop.<Initialize>d__35&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Activity_Monopoly.<>c__DisplayClass101_1.<<OnRollDiceResponse>b__0>d>(XFramework.UIPanel_Activity_Monopoly.<>c__DisplayClass101_1.<<OnRollDiceResponse>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Activity_Monopoly.<>c__DisplayClass87_0.<<InitNode>b__5>d>(XFramework.UIPanel_Activity_Monopoly.<>c__DisplayClass87_0.<<InitNode>b__5>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Activity_Monopoly.<>c__DisplayClass87_0.<<InitNode>b__7>d>(XFramework.UIPanel_Activity_Monopoly.<>c__DisplayClass87_0.<<InitNode>b__7>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Activity_Monopoly.<InitGridList>d__90>(XFramework.UIPanel_Activity_Monopoly.<InitGridList>d__90&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Activity_Monopoly.<InitNode>d__87>(XFramework.UIPanel_Activity_Monopoly.<InitNode>d__87&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Activity_Monopoly.<Initialize>d__84>(XFramework.UIPanel_Activity_Monopoly.<Initialize>d__84&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Activity_Monopoly.<OnQueryMonopolyTaskResponse>d__100>(XFramework.UIPanel_Activity_Monopoly.<OnQueryMonopolyTaskResponse>d__100&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Activity_Monopoly.<OnRollDiceResponse>d__101>(XFramework.UIPanel_Activity_Monopoly.<OnRollDiceResponse>d__101&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Activity_NewSign.<InitNode>d__32>(XFramework.UIPanel_Activity_NewSign.<InitNode>d__32&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Activity_NewSign.<Initialize>d__30>(XFramework.UIPanel_Activity_NewSign.<Initialize>d__30&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Activity_NewSign.<OnInitNewSignResponse>d__33>(XFramework.UIPanel_Activity_NewSign.<OnInitNewSignResponse>d__33&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_AnimTools.<InitNode>d__17>(XFramework.UIPanel_AnimTools.<InitNode>d__17&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Bank.<Initialize>d__35>(XFramework.UIPanel_Bank.<Initialize>d__35&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_BattleDamageInfo.<<InitNode>b__10_1>d>(XFramework.UIPanel_BattleDamageInfo.<<InitNode>b__10_1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_BattleDamageInfo.<InitNode>d__10>(XFramework.UIPanel_BattleDamageInfo.<InitNode>d__10&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_BattleDamageInfo.<Initialize>d__9>(XFramework.UIPanel_BattleDamageInfo.<Initialize>d__9&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_BattleInfo.<Initialize>d__58>(XFramework.UIPanel_BattleInfo.<Initialize>d__58&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_BattleShop.<>c__DisplayClass67_0.<<InitPanel>b__0>d>(XFramework.UIPanel_BattleShop.<>c__DisplayClass67_0.<<InitPanel>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_BattleShop.<>c__DisplayClass79_1.<<CreateSkillsItem>b__4>d>(XFramework.UIPanel_BattleShop.<>c__DisplayClass79_1.<<CreateSkillsItem>b__4>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_BattleShop.<Initialize>d__64>(XFramework.UIPanel_BattleShop.<Initialize>d__64&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_BattleTecnology.<<Initialize>b__30_1>d>(XFramework.UIPanel_BattleTecnology.<<Initialize>b__30_1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_BattleTecnology.<Initialize>d__30>(XFramework.UIPanel_BattleTecnology.<Initialize>d__30&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_BuyDice.<>c__DisplayClass26_0.<<InitNode>b__5>d>(XFramework.UIPanel_BuyDice.<>c__DisplayClass26_0.<<InitNode>b__5>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_BuyDice.<InitNode>d__26>(XFramework.UIPanel_BuyDice.<InitNode>d__26&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_BuyDice.<OnBuyDiceResponse>d__27>(XFramework.UIPanel_BuyDice.<OnBuyDiceResponse>d__27&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_BuyEnergy.<Initialize>d__39>(XFramework.UIPanel_BuyEnergy.<Initialize>d__39&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_BuyEnergy.<OnBuyEnergyResponse>d__47>(XFramework.UIPanel_BuyEnergy.<OnBuyEnergyResponse>d__47&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_BuyEnergy.<UpdateTimeView>d__52>(XFramework.UIPanel_BuyEnergy.<UpdateTimeView>d__52&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Challege.<CreateAreaTreadAreaInfo>d__71>(XFramework.UIPanel_Challege.<CreateAreaTreadAreaInfo>d__71&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Challege.<CreateEventDes>d__84>(XFramework.UIPanel_Challege.<CreateEventDes>d__84&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Challege.<OnChallengeButtonClick>d__77>(XFramework.UIPanel_Challege.<OnChallengeButtonClick>d__77&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Challege.<OnLoopClick>d__65>(XFramework.UIPanel_Challege.<OnLoopClick>d__65&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Challege.<OnMainChallengeAreaClick>d__81>(XFramework.UIPanel_Challege.<OnMainChallengeAreaClick>d__81&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Compound.<>c__DisplayClass39_0.<<InitPanel>b__3>d>(XFramework.UIPanel_Compound.<>c__DisplayClass39_0.<<InitPanel>b__3>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Compound.<>c__DisplayClass39_0.<<InitPanel>b__4>d>(XFramework.UIPanel_Compound.<>c__DisplayClass39_0.<<InitPanel>b__4>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Compound.<>c__DisplayClass39_0.<<InitPanel>b__5>d>(XFramework.UIPanel_Compound.<>c__DisplayClass39_0.<<InitPanel>b__5>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Compound.<>c__DisplayClass44_0.<<SetSuccess>b__0>d>(XFramework.UIPanel_Compound.<>c__DisplayClass44_0.<<SetSuccess>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Compound.<OnEquipAllCompoundResponse>d__55>(XFramework.UIPanel_Compound.<OnEquipAllCompoundResponse>d__55&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_CompoundDongHua.<<SetSuccess>b__27_0>d>(XFramework.UIPanel_CompoundDongHua.<<SetSuccess>b__27_0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_CompoundDongHua.<ClosePanel>d__31>(XFramework.UIPanel_CompoundDongHua.<ClosePanel>d__31&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_CompoundDongHua.<Initialize>d__26>(XFramework.UIPanel_CompoundDongHua.<Initialize>d__26&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_CompoundDongHua.<OnEquipAllCompoundResponse>d__37>(XFramework.UIPanel_CompoundDongHua.<OnEquipAllCompoundResponse>d__37&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_EquipDownGrade.<>c__DisplayClass22_0.<<InitPanel>b__1>d>(XFramework.UIPanel_EquipDownGrade.<>c__DisplayClass22_0.<<InitPanel>b__1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_EquipDownGrade.<Initialize>d__19>(XFramework.UIPanel_EquipDownGrade.<Initialize>d__19&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_EquipTips.<Initialize>d__37>(XFramework.UIPanel_EquipTips.<Initialize>d__37&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Equipment.<>c__DisplayClass65_1.<<InitTab2WidegetInfo>b__0>d>(XFramework.UIPanel_Equipment.<>c__DisplayClass65_1.<<InitTab2WidegetInfo>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Fail.<InitReWardItem>d__21>(XFramework.UIPanel_Fail.<InitReWardItem>d__21&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Fail.<Initialize>d__18>(XFramework.UIPanel_Fail.<Initialize>d__18&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_First_Charge.<Initialize>d__15>(XFramework.UIPanel_First_Charge.<Initialize>d__15&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_GuideTips.<InitNode>d__12>(XFramework.UIPanel_GuideTips.<InitNode>d__12&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_JiyuGame.<Initialize>d__35>(XFramework.UIPanel_JiyuGame.<Initialize>d__35&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_JiyuGame.<Update>d__64>(XFramework.UIPanel_JiyuGame.<Update>d__64&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Login.<>c__DisplayClass9_0.<<OnLoginResponse>b__0>d>(XFramework.UIPanel_Login.<>c__DisplayClass9_0.<<OnLoginResponse>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Login.<OnLoginResponse>d__9>(XFramework.UIPanel_Login.<OnLoginResponse>d__9&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Login.<OnQueryLoginSettingsResponse>d__8>(XFramework.UIPanel_Login.<OnQueryLoginSettingsResponse>d__8&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Mail.<<Initialize>b__38_0>d>(XFramework.UIPanel_Mail.<<Initialize>b__38_0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Mail.<<Initialize>b__38_1>d>(XFramework.UIPanel_Mail.<<Initialize>b__38_1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Mail.<InitTopContent>d__45>(XFramework.UIPanel_Mail.<InitTopContent>d__45&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Mail.<Initialize>d__38>(XFramework.UIPanel_Mail.<Initialize>d__38&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Main.<>c__DisplayClass89_0.<<InitNode>b__0>d>(XFramework.UIPanel_Main.<>c__DisplayClass89_0.<<InitNode>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Main.<>c__DisplayClass89_11.<<InitNode>b__36>d>(XFramework.UIPanel_Main.<>c__DisplayClass89_11.<<InitNode>b__36>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Main.<>c__DisplayClass89_8.<<InitNode>b__29>d>(XFramework.UIPanel_Main.<>c__DisplayClass89_8.<<InitNode>b__29>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Main.<>c__DisplayClass89_8.<<InitNode>b__31>d>(XFramework.UIPanel_Main.<>c__DisplayClass89_8.<<InitNode>b__31>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Main.<>c__DisplayClass89_9.<<InitNode>b__32>d>(XFramework.UIPanel_Main.<>c__DisplayClass89_9.<<InitNode>b__32>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Main.<OnChangeNameStatusResponse>d__94>(XFramework.UIPanel_Main.<OnChangeNameStatusResponse>d__94&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Main.<OnClickTagFunc>d__111>(XFramework.UIPanel_Main.<OnClickTagFunc>d__111&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Main.<OnGuideIdFinished>d__91>(XFramework.UIPanel_Main.<OnGuideIdFinished>d__91&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Main.<OnQueryMonopolyTaskResponse>d__99>(XFramework.UIPanel_Main.<OnQueryMonopolyTaskResponse>d__99&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Main.<OnStartButtonClick>d__129>(XFramework.UIPanel_Main.<OnStartButtonClick>d__129&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Main.<OpenPlayerInfo>d__119>(XFramework.UIPanel_Main.<OpenPlayerInfo>d__119&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Main.<UpdateTimeView>d__116>(XFramework.UIPanel_Main.<UpdateTimeView>d__116&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_MonopolyShop.<Initialize>d__38>(XFramework.UIPanel_MonopolyShop.<Initialize>d__38&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_MonopolyShop.<OnExchangeResponse>d__41>(XFramework.UIPanel_MonopolyShop.<OnExchangeResponse>d__41&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_MonopolyShop.<ProvideData>d__47>(XFramework.UIPanel_MonopolyShop.<ProvideData>d__47&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_MonopolyTaskShop.<Initialize>d__41>(XFramework.UIPanel_MonopolyTaskShop.<Initialize>d__41&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_MonopolyTaskShop.<OnGetTaskResponse>d__44>(XFramework.UIPanel_MonopolyTaskShop.<OnGetTaskResponse>d__44&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_MonsterCollection.<>c__DisplayClass37_2.<<ProvideData>b__5>d>(XFramework.UIPanel_MonsterCollection.<>c__DisplayClass37_2.<<ProvideData>b__5>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_MonsterCollection.<>c__DisplayClass37_3.<<ProvideData>b__7>d>(XFramework.UIPanel_MonsterCollection.<>c__DisplayClass37_3.<<ProvideData>b__7>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_MonsterCollection.<>c__DisplayClass37_5.<<ProvideData>b__15>d>(XFramework.UIPanel_MonsterCollection.<>c__DisplayClass37_5.<<ProvideData>b__15>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_MonsterCollection.<>c__DisplayClass37_6.<<ProvideData>b__17>d>(XFramework.UIPanel_MonsterCollection.<>c__DisplayClass37_6.<<ProvideData>b__17>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_MonsterCollection.<ProvideData>d__37>(XFramework.UIPanel_MonsterCollection.<ProvideData>d__37&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Notice.<<Initialize>b__27_0>d>(XFramework.UIPanel_Notice.<<Initialize>b__27_0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Notice.<<Initialize>b__27_1>d>(XFramework.UIPanel_Notice.<<Initialize>b__27_1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Notice.<Initialize>d__27>(XFramework.UIPanel_Notice.<Initialize>d__27&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_ParasTest.<<Initialize>b__24_5>d>(XFramework.UIPanel_ParasTest.<<Initialize>b__24_5>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_ParasTest.<Initialize>d__24>(XFramework.UIPanel_ParasTest.<Initialize>d__24&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Pass.<<BtnInit>b__61_0>d>(XFramework.UIPanel_Pass.<<BtnInit>b__61_0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Pass.<<BtnInit>b__61_1>d>(XFramework.UIPanel_Pass.<<BtnInit>b__61_1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Pass.<<CreateItem>b__80_4>d>(XFramework.UIPanel_Pass.<<CreateItem>b__80_4>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Pass.<>c__DisplayClass74_0.<<BoxSet>b__0>d>(XFramework.UIPanel_Pass.<>c__DisplayClass74_0.<<BoxSet>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Pass.<Initialize>d__51>(XFramework.UIPanel_Pass.<Initialize>d__51&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Patrol.<<InitWidgetAction>b__71_0>d>(XFramework.UIPanel_Patrol.<<InitWidgetAction>b__71_0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Patrol.<CreateAutoAward>d__91>(XFramework.UIPanel_Patrol.<CreateAutoAward>d__91&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Patrol.<CreateRapidAward>d__89>(XFramework.UIPanel_Patrol.<CreateRapidAward>d__89&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Patrol.<Initialize>d__68>(XFramework.UIPanel_Patrol.<Initialize>d__68&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Person.<Btn52OnClick>d__61>(XFramework.UIPanel_Person.<Btn52OnClick>d__61&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Person.<CreatePrompt>d__53>(XFramework.UIPanel_Person.<CreatePrompt>d__53&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Person.<HeadImgInit>d__63>(XFramework.UIPanel_Person.<HeadImgInit>d__63&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Person.<OnChangeNameStatusResponse>d__52>(XFramework.UIPanel_Person.<OnChangeNameStatusResponse>d__52&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Quest.<Initialize>d__10>(XFramework.UIPanel_Quest.<Initialize>d__10&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Rebirth.<>c__DisplayClass19_0.<<InitNode>b__0>d>(XFramework.UIPanel_Rebirth.<>c__DisplayClass19_0.<<InitNode>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Rebirth.<>c__DisplayClass19_0.<<InitNode>b__1>d>(XFramework.UIPanel_Rebirth.<>c__DisplayClass19_0.<<InitNode>b__1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Rebirth.<>c__DisplayClass19_0.<<InitNode>b__2>d>(XFramework.UIPanel_Rebirth.<>c__DisplayClass19_0.<<InitNode>b__2>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Rebirth.<InitNode>d__19>(XFramework.UIPanel_Rebirth.<InitNode>d__19&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Rebirth.<Initialize>d__17>(XFramework.UIPanel_Rebirth.<Initialize>d__17&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_ReturnConfirm.<<InitNode>b__10_0>d>(XFramework.UIPanel_ReturnConfirm.<<InitNode>b__10_0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_ReturnConfirm.<<InitNode>b__10_1>d>(XFramework.UIPanel_ReturnConfirm.<<InitNode>b__10_1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_ReturnConfirm.<Initialize>d__8>(XFramework.UIPanel_ReturnConfirm.<Initialize>d__8&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_RunTimeHUD.<InitGuidScene>d__73>(XFramework.UIPanel_RunTimeHUD.<InitGuidScene>d__73&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_RunTimeHUD.<Initialize>d__69>(XFramework.UIPanel_RunTimeHUD.<Initialize>d__69&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_RunTimeHUD.<Initialize>d__80>(XFramework.UIPanel_RunTimeHUD.<Initialize>d__80&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_RunTimeHUD.<OnGuideOrderFinished>d__75>(XFramework.UIPanel_RunTimeHUD.<OnGuideOrderFinished>d__75&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_RunTimeHUD.<Update>d__94>(XFramework.UIPanel_RunTimeHUD.<Update>d__94&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_SelectBoxNomal.<>c__DisplayClass28_1.<<InitNode>b__11>d>(XFramework.UIPanel_SelectBoxNomal.<>c__DisplayClass28_1.<<InitNode>b__11>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_SelectBoxNomal.<OnSelfChooseBoxResponse>d__29>(XFramework.UIPanel_SelectBoxNomal.<OnSelfChooseBoxResponse>d__29&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Settings.<<OnClickEvent>b__46_0>d>(XFramework.UIPanel_Settings.<<OnClickEvent>b__46_0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Settings.<<OnClickEvent>b__46_1>d>(XFramework.UIPanel_Settings.<<OnClickEvent>b__46_1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Settings.<>c.<<Init>b__43_10>d>(XFramework.UIPanel_Settings.<>c.<<Init>b__43_10>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Settings.<>c.<<Init>b__43_8>d>(XFramework.UIPanel_Settings.<>c.<<Init>b__43_8>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Settings.<Initialize>d__41>(XFramework.UIPanel_Settings.<Initialize>d__41&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Share.<Initialize>d__25>(XFramework.UIPanel_Share.<Initialize>d__25&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Shop.<Module1101>d__102>(XFramework.UIPanel_Shop.<Module1101>d__102&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Shop.<Module1404>d__141>(XFramework.UIPanel_Shop.<Module1404>d__141&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Sign.<Initialize>d__34>(XFramework.UIPanel_Sign.<Initialize>d__34&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Sign.<OnBigResponse>d__49>(XFramework.UIPanel_Sign.<OnBigResponse>d__49&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Sign.<OnDailResponse>d__46>(XFramework.UIPanel_Sign.<OnDailResponse>d__46&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Sign.<QuertInit>d__35>(XFramework.UIPanel_Sign.<QuertInit>d__35&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Sign.<SetState>d__43>(XFramework.UIPanel_Sign.<SetState>d__43&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Success.<InitReWardItem>d__19>(XFramework.UIPanel_Success.<InitReWardItem>d__19&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Success.<Initialize>d__15>(XFramework.UIPanel_Success.<Initialize>d__15&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Sweep.<<InitBtn>b__65_0>d>(XFramework.UIPanel_Sweep.<<InitBtn>b__65_0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Sweep.<Initialize>d__59>(XFramework.UIPanel_Sweep.<Initialize>d__59&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Sweep.<OnBtnGetClick>d__70>(XFramework.UIPanel_Sweep.<OnBtnGetClick>d__70&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Sweep.<UpdateCardState>d__62>(XFramework.UIPanel_Sweep.<UpdateCardState>d__62&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Talent.<ActPlyarAnimation>d__55>(XFramework.UIPanel_Talent.<ActPlyarAnimation>d__55&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Talent.<SetTanlentPropDetails>d__78>(XFramework.UIPanel_Talent.<SetTanlentPropDetails>d__78&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Talent_Prop.<Initialize>d__11>(XFramework.UIPanel_Talent_Prop.<Initialize>d__11&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Talent_Prop.<ProvideData>d__17>(XFramework.UIPanel_Talent_Prop.<ProvideData>d__17&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Task_DailyAndWeekly.<Initialize>d__38>(XFramework.UIPanel_Task_DailyAndWeekly.<Initialize>d__38&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Task_DailyAndWeekly.<OnCliamScoreBoxResponse>d__61>(XFramework.UIPanel_Task_DailyAndWeekly.<OnCliamScoreBoxResponse>d__61&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Task_DailyAndWeekly.<OnCliamTaskResponse>d__49>(XFramework.UIPanel_Task_DailyAndWeekly.<OnCliamTaskResponse>d__49&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_Task_DailyAndWeekly.<OnGetAllDailyResponse>d__45>(XFramework.UIPanel_Task_DailyAndWeekly.<OnGetAllDailyResponse>d__45&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPanel_UnlockBlock.<Initialize>d__19>(XFramework.UIPanel_UnlockBlock.<Initialize>d__19&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPlayerInformtion.<>c__DisplayClass41_0.<<Initialize>b__2>d>(XFramework.UIPlayerInformtion.<>c__DisplayClass41_0.<<Initialize>b__2>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPlayerInformtion.<>c__DisplayClass41_0.<<Initialize>b__3>d>(XFramework.UIPlayerInformtion.<>c__DisplayClass41_0.<<Initialize>b__3>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPlayerInformtion.<>c__DisplayClass41_0.<<Initialize>b__4>d>(XFramework.UIPlayerInformtion.<>c__DisplayClass41_0.<<Initialize>b__4>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPlayerInformtion.<>c__DisplayClass41_0.<<Initialize>b__5>d>(XFramework.UIPlayerInformtion.<>c__DisplayClass41_0.<<Initialize>b__5>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPlayerInformtion.<InitFrameItem>d__45>(XFramework.UIPlayerInformtion.<InitFrameItem>d__45&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPlayerInformtion.<InitHeadItem>d__44>(XFramework.UIPlayerInformtion.<InitHeadItem>d__44&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPlayerInformtion.<Initialize>d__41>(XFramework.UIPlayerInformtion.<Initialize>d__41&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIPlayerInformtion.<OnChangeNameStatusResponse>d__48>(XFramework.UIPlayerInformtion.<OnChangeNameStatusResponse>d__48&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIResource.<>c__DisplayClass2_0.<<ResourceAni>b__2>d>(XFramework.UIResource.<>c__DisplayClass2_0.<<ResourceAni>b__2>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIResource.<>c__DisplayClass2_1.<<ResourceAni>b__0>d>(XFramework.UIResource.<>c__DisplayClass2_1.<<ResourceAni>b__0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIResource.<>c__DisplayClass2_1.<<ResourceAni>b__1>d>(XFramework.UIResource.<>c__DisplayClass2_1.<<ResourceAni>b__1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UIResource.<ResourceAni>d__2>(XFramework.UIResource.<ResourceAni>d__2&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISettlementItem.<Initialize>d__5>(XFramework.UISettlementItem.<Initialize>d__5&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Equipment.<OnBtnClickEvent>d__18>(XFramework.UISubPanel_Equipment.<OnBtnClickEvent>d__18&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Equipment.<OnLoopClick>d__20>(XFramework.UISubPanel_Equipment.<OnLoopClick>d__20&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_MailItem.<OnReceiveGift>d__20>(XFramework.UISubPanel_MailItem.<OnReceiveGift>d__20&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Pass_Token.<Initialize>d__18>(XFramework.UISubPanel_Pass_Token.<Initialize>d__18&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Pass_Token_Item.<Initialize>d__3>(XFramework.UISubPanel_Pass_Token_Item.<Initialize>d__3&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Person_ChangeName.<CreatePrompt>d__41>(XFramework.UISubPanel_Person_ChangeName.<CreatePrompt>d__41&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Person_ChangeName.<Initialize>d__32>(XFramework.UISubPanel_Person_ChangeName.<Initialize>d__32&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Person_ChangeName.<SelectStatus>d__38>(XFramework.UISubPanel_Person_ChangeName.<SelectStatus>d__38&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Person_ChangeNameCenter.<CreatePrompt>d__40>(XFramework.UISubPanel_Person_ChangeNameCenter.<CreatePrompt>d__40&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Person_ChangeNameCenter.<Initialize>d__31>(XFramework.UISubPanel_Person_ChangeNameCenter.<Initialize>d__31&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Person_ChangeNameCenter.<SelectStatus>d__37>(XFramework.UISubPanel_Person_ChangeNameCenter.<SelectStatus>d__37&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Person_UserInfo.<BottomInit>d__36>(XFramework.UISubPanel_Person_UserInfo.<BottomInit>d__36&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Person_UserInfo.<FrameImageSet>d__33>(XFramework.UISubPanel_Person_UserInfo.<FrameImageSet>d__33&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Person_UserInfo.<FrameSet>d__40>(XFramework.UISubPanel_Person_UserInfo.<FrameSet>d__40&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Person_UserInfo.<HeadImageSet>d__31>(XFramework.UISubPanel_Person_UserInfo.<HeadImageSet>d__31&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Person_UserInfo.<HeadImgInit>d__38>(XFramework.UISubPanel_Person_UserInfo.<HeadImgInit>d__38&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Person_UserInfo.<HeadSet>d__39>(XFramework.UISubPanel_Person_UserInfo.<HeadSet>d__39&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Person_UserInfo.<OnChangeNameStatusResponse>d__45>(XFramework.UISubPanel_Person_UserInfo.<OnChangeNameStatusResponse>d__45&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_1102_SBox.<BtnTxtInit>d__59>(XFramework.UISubPanel_Shop_1102_SBox.<BtnTxtInit>d__59&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_1103_Box.<TxtInit>d__42>(XFramework.UISubPanel_Shop_1103_Box.<TxtInit>d__42&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_1103_Box.Type1WithKey.<ChangeBtnState>d__0>(XFramework.UISubPanel_Shop_1103_Box.Type1WithKey.<ChangeBtnState>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_1103_Box.Type1WithoutKey.<ChangeBtnState>d__0>(XFramework.UISubPanel_Shop_1103_Box.Type1WithoutKey.<ChangeBtnState>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_1103_Box.Type2WithKey.<ChangeBtnState>d__0>(XFramework.UISubPanel_Shop_1103_Box.Type2WithKey.<ChangeBtnState>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_1103_Box.Type2WithoutKey.<ChangeBtnState>d__0>(XFramework.UISubPanel_Shop_1103_Box.Type2WithoutKey.<ChangeBtnState>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_1103_Box.Type4KeyFew.<ChangeBtnState>d__0>(XFramework.UISubPanel_Shop_1103_Box.Type4KeyFew.<ChangeBtnState>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_1103_Box.Type4KeyMany.<ChangeBtnState>d__0>(XFramework.UISubPanel_Shop_1103_Box.Type4KeyMany.<ChangeBtnState>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_1103_Box.Type4OnlyKey.<ChangeBtnState>d__0>(XFramework.UISubPanel_Shop_1103_Box.Type4OnlyKey.<ChangeBtnState>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_1103_Box.Type4WithOutKey.<ChangeBtnState>d__0>(XFramework.UISubPanel_Shop_1103_Box.Type4WithOutKey.<ChangeBtnState>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_1103_Box.Type5HaveAdvertWithKey.<ChangeBtnState>d__0>(XFramework.UISubPanel_Shop_1103_Box.Type5HaveAdvertWithKey.<ChangeBtnState>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_1103_Box.Type5HaveAdvertWithoutKey.<ChangeBtnState>d__0>(XFramework.UISubPanel_Shop_1103_Box.Type5HaveAdvertWithoutKey.<ChangeBtnState>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_1103_Box.Type5NoAdvertWithKey.<ChangeBtnState>d__0>(XFramework.UISubPanel_Shop_1103_Box.Type5NoAdvertWithKey.<ChangeBtnState>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_1103_Box.Type5NoAdvertWithoutKey.<ChangeBtnState>d__0>(XFramework.UISubPanel_Shop_1103_Box.Type5NoAdvertWithoutKey.<ChangeBtnState>d__0&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_1403_Fund.<Module1403_Help_CreateItem>d__52>(XFramework.UISubPanel_Shop_1403_Fund.<Module1403_Help_CreateItem>d__52&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_Draw.<<Initialize>b__45_0>d>(XFramework.UISubPanel_Shop_Draw.<<Initialize>b__45_0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_Draw.<DisplayInit>d__46>(XFramework.UISubPanel_Shop_Draw.<DisplayInit>d__46&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_Fund_List.<Initialize>d__24>(XFramework.UISubPanel_Shop_Fund_List.<Initialize>d__24&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_Pre.<BottomInit>d__24>(XFramework.UISubPanel_Shop_Pre.<BottomInit>d__24&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_Pre.<CreateTipHelp>d__28>(XFramework.UISubPanel_Shop_Pre.<CreateTipHelp>d__28&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_Pre.<InScreenOrNot>d__29>(XFramework.UISubPanel_Shop_Pre.<InScreenOrNot>d__29&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_Pre.<Initialize>d__22>(XFramework.UISubPanel_Shop_Pre.<Initialize>d__22&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.UISubPanel_Shop_item.<>c__DisplayClass51_0.<<Initialize>b__5>d>(XFramework.UISubPanel_Shop_item.<>c__DisplayClass51_0.<<Initialize>b__5>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<XFramework.XFEntry.<Init>d__5>(XFramework.XFEntry.<Init>d__5&)
		// object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<object>(object&)
		// System.Threading.Tasks.Task<object> System.Threading.Tasks.Task.Run<object>(System.Func<object>)
		// long Unity.Burst.BurstRuntime.GetHashCode64<object>()
		// System.Void* Unity.Collections.AllocatorManager.Allocate<Unity.Collections.AllocatorManager.AllocatorHandle>(Unity.Collections.AllocatorManager.AllocatorHandle&,int,int,int)
		// Unity.Collections.AllocatorManager.Block Unity.Collections.AllocatorManager.AllocateBlock<Unity.Collections.AllocatorManager.AllocatorHandle>(Unity.Collections.AllocatorManager.AllocatorHandle&,int,int,int)
		// System.Void* Unity.Collections.AllocatorManager.AllocateStruct<Unity.Collections.AllocatorManager.AllocatorHandle,Main.ChaStats>(Unity.Collections.AllocatorManager.AllocatorHandle&,Main.ChaStats,int)
		// System.Void* Unity.Collections.AllocatorManager.AllocateStruct<Unity.Collections.AllocatorManager.AllocatorHandle,Main.PlayerData>(Unity.Collections.AllocatorManager.AllocatorHandle&,Main.PlayerData,int)
		// System.Void* Unity.Collections.AllocatorManager.AllocateStruct<Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Transforms.LocalTransform>(Unity.Collections.AllocatorManager.AllocatorHandle&,Unity.Transforms.LocalTransform,int)
		// Unity.Collections.NativeArray<Main.ChaStats> Unity.Collections.CollectionHelper.CreateNativeArray<Main.ChaStats>(int,Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Collections.NativeArrayOptions)
		// Unity.Collections.NativeArray<Main.PlayerData> Unity.Collections.CollectionHelper.CreateNativeArray<Main.PlayerData>(int,Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Collections.NativeArrayOptions)
		// Unity.Collections.NativeArray<Unity.Transforms.LocalTransform> Unity.Collections.CollectionHelper.CreateNativeArray<Unity.Transforms.LocalTransform>(int,Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Collections.NativeArrayOptions)
		// Unity.Collections.CopyError Unity.Collections.FixedStringMethods.CopyFromTruncated<Unity.Collections.FixedString64Bytes,Unity.Collections.FixedString128Bytes>(Unity.Collections.FixedString64Bytes&,Unity.Collections.FixedString128Bytes&)
		// System.Void* Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.GetUnsafePtr<Main.ChaStats>(Unity.Collections.NativeArray<Main.ChaStats>)
		// System.Void* Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.GetUnsafePtr<Main.PlayerData>(Unity.Collections.NativeArray<Main.PlayerData>)
		// System.Void* Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.GetUnsafePtr<Unity.Transforms.LocalTransform>(Unity.Collections.NativeArray<Unity.Transforms.LocalTransform>)
		// int* Unity.Collections.LowLevel.Unsafe.NativeListUnsafeUtility.GetUnsafeReadOnlyPtr<int>(Unity.Collections.NativeList<int>)
		// System.Void* Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AddressOf<Main.HybridEventData>(Main.HybridEventData&)
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AlignOf<Main.ChaStats>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AlignOf<Main.PlayerData>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AlignOf<Unity.Transforms.LocalTransform>()
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<Main.ChaStats>(System.Void*,Main.ChaStats&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<Main.DropsData>(System.Void*,Main.DropsData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<Main.EnemyData>(System.Void*,Main.EnemyData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<Main.EntityGroupData>(System.Void*,Main.EntityGroupData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<Main.EnviromentData>(System.Void*,Main.EnviromentData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<Main.GameOthersData>(System.Void*,Main.GameOthersData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<Main.GameSetUpData>(System.Void*,Main.GameSetUpData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<Main.GameTimeData>(System.Void*,Main.GameTimeData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<Main.GlobalConfigData>(System.Void*,Main.GlobalConfigData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<Main.HybridEventData>(System.Void*,Main.HybridEventData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<Main.IntroGuideItemData>(System.Void*,Main.IntroGuideItemData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<Main.PlayerData>(System.Void*,Main.PlayerData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<Main.PrefabMapData>(System.Void*,Main.PrefabMapData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<Main.SpineHybridData>(System.Void*,Main.SpineHybridData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<Main.StateMachine>(System.Void*,Main.StateMachine&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<Main.WeaponData>(System.Void*,Main.WeaponData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<ProjectDawn.Navigation.AgentBody>(System.Void*,ProjectDawn.Navigation.AgentBody&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<ProjectDawn.Navigation.AgentLocomotion>(System.Void*,ProjectDawn.Navigation.AgentLocomotion&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<ProjectDawn.Navigation.AgentShape>(System.Void*,ProjectDawn.Navigation.AgentShape&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<Unity.Physics.PhysicsMass>(System.Void*,Unity.Physics.PhysicsMass&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyPtrToStructure<Unity.Transforms.LocalTransform>(System.Void*,Unity.Transforms.LocalTransform&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Main.ChaStats>(Main.ChaStats&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Main.DropsData>(Main.DropsData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Main.EntityGroupData>(Main.EntityGroupData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Main.EnviromentData>(Main.EnviromentData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Main.GameCameraSizeData>(Main.GameCameraSizeData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Main.GameOthersData>(Main.GameOthersData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Main.GameRandomData>(Main.GameRandomData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Main.GameSetUpData>(Main.GameSetUpData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Main.GameTimeData>(Main.GameTimeData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Main.GlobalConfigData>(Main.GlobalConfigData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Main.HybridEventData>(Main.HybridEventData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Main.MapRefreshData>(Main.MapRefreshData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Main.PlayerData>(Main.PlayerData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Main.PushColliderData>(Main.PushColliderData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Main.SpineHybridData>(Main.SpineHybridData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Main.StateMachine>(Main.StateMachine&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Main.WeaponData>(Main.WeaponData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<ProjectDawn.Navigation.AgentBody>(ProjectDawn.Navigation.AgentBody&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<ProjectDawn.Navigation.AgentLocomotion>(ProjectDawn.Navigation.AgentLocomotion&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<ProjectDawn.Navigation.AgentShape>(ProjectDawn.Navigation.AgentShape&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Unity.Physics.PhysicsMass>(Unity.Physics.PhysicsMass&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Unity.Transforms.LocalTransform>(Unity.Transforms.LocalTransform&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.CopyStructureToPtr<Unity.Transforms.Parent>(Unity.Transforms.Parent&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<Main.ChaStats>(System.Void*,Main.ChaStats&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<Main.DropsData>(System.Void*,Main.DropsData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<Main.EnemyData>(System.Void*,Main.EnemyData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<Main.EntityGroupData>(System.Void*,Main.EntityGroupData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<Main.EnviromentData>(System.Void*,Main.EnviromentData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<Main.GameOthersData>(System.Void*,Main.GameOthersData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<Main.GameSetUpData>(System.Void*,Main.GameSetUpData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<Main.GameTimeData>(System.Void*,Main.GameTimeData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<Main.GlobalConfigData>(System.Void*,Main.GlobalConfigData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<Main.HybridEventData>(System.Void*,Main.HybridEventData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<Main.IntroGuideItemData>(System.Void*,Main.IntroGuideItemData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<Main.PlayerData>(System.Void*,Main.PlayerData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<Main.PrefabMapData>(System.Void*,Main.PrefabMapData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<Main.SpineHybridData>(System.Void*,Main.SpineHybridData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<Main.StateMachine>(System.Void*,Main.StateMachine&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<Main.WeaponData>(System.Void*,Main.WeaponData&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<ProjectDawn.Navigation.AgentBody>(System.Void*,ProjectDawn.Navigation.AgentBody&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<ProjectDawn.Navigation.AgentLocomotion>(System.Void*,ProjectDawn.Navigation.AgentLocomotion&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<ProjectDawn.Navigation.AgentShape>(System.Void*,ProjectDawn.Navigation.AgentShape&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<Unity.Physics.PhysicsMass>(System.Void*,Unity.Physics.PhysicsMass&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyPtrToStructure<Unity.Transforms.LocalTransform>(System.Void*,Unity.Transforms.LocalTransform&)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Main.ChaStats>(Main.ChaStats&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Main.DropsData>(Main.DropsData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Main.EntityGroupData>(Main.EntityGroupData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Main.EnviromentData>(Main.EnviromentData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Main.GameCameraSizeData>(Main.GameCameraSizeData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Main.GameOthersData>(Main.GameOthersData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Main.GameRandomData>(Main.GameRandomData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Main.GameSetUpData>(Main.GameSetUpData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Main.GameTimeData>(Main.GameTimeData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Main.GlobalConfigData>(Main.GlobalConfigData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Main.HybridEventData>(Main.HybridEventData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Main.MapRefreshData>(Main.MapRefreshData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Main.PlayerData>(Main.PlayerData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Main.PushColliderData>(Main.PushColliderData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Main.SpineHybridData>(Main.SpineHybridData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Main.StateMachine>(Main.StateMachine&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Main.WeaponData>(Main.WeaponData&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<ProjectDawn.Navigation.AgentBody>(ProjectDawn.Navigation.AgentBody&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<ProjectDawn.Navigation.AgentLocomotion>(ProjectDawn.Navigation.AgentLocomotion&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<ProjectDawn.Navigation.AgentShape>(ProjectDawn.Navigation.AgentShape&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Unity.Physics.PhysicsMass>(Unity.Physics.PhysicsMass&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Unity.Transforms.LocalTransform>(Unity.Transforms.LocalTransform&,System.Void*)
		// System.Void Unity.Collections.LowLevel.Unsafe.UnsafeUtility.InternalCopyStructureToPtr<Unity.Transforms.Parent>(Unity.Transforms.Parent&,System.Void*)
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.ReadArrayElement<int>(System.Void*,int)
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<Main.ChaStats>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<Main.GameOthersData>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<Main.HybridEventData>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<Main.PlayerData>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AlignOfHelper<Main.ChaStats>>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AlignOfHelper<Main.PlayerData>>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AlignOfHelper<Unity.Transforms.LocalTransform>>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<Unity.Transforms.LocalTransform>()
		// int Unity.Collections.NativeArrayExtensions.IndexOf<int,int>(System.Void*,int,int)
		// System.Void Unity.Collections.NativeArrayExtensions.Initialize<Main.ChaStats>(Unity.Collections.NativeArray<Main.ChaStats>&,int,Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Collections.NativeArrayOptions)
		// System.Void Unity.Collections.NativeArrayExtensions.Initialize<Main.PlayerData>(Unity.Collections.NativeArray<Main.PlayerData>&,int,Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Collections.NativeArrayOptions)
		// System.Void Unity.Collections.NativeArrayExtensions.Initialize<Unity.Transforms.LocalTransform>(Unity.Collections.NativeArray<Unity.Transforms.LocalTransform>&,int,Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Collections.NativeArrayOptions)
		// bool Unity.Collections.NativeListExtensions.Contains<int,int>(Unity.Collections.NativeList<int>,int)
		// Unity.Collections.NativeArray<Main.ChaStats> Unity.Entities.ChunkIterationUtility.CreateComponentDataArray<Main.ChaStats>(Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Entities.ComponentTypeHandle<Main.ChaStats>&,int,Unity.Entities.EntityQuery)
		// Unity.Collections.NativeArray<Main.PlayerData> Unity.Entities.ChunkIterationUtility.CreateComponentDataArray<Main.PlayerData>(Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Entities.ComponentTypeHandle<Main.PlayerData>&,int,Unity.Entities.EntityQuery)
		// Unity.Collections.NativeArray<Unity.Transforms.LocalTransform> Unity.Entities.ChunkIterationUtility.CreateComponentDataArray<Unity.Transforms.LocalTransform>(Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Entities.ComponentTypeHandle<Unity.Transforms.LocalTransform>&,int,Unity.Entities.EntityQuery)
		// System.Void Unity.Entities.ComponentSystemBase.RequireForUpdate<Main.HybridEventData>()
		// System.Void Unity.Entities.ComponentSystemBase.RequireForUpdate<Main.PlayerData>()
		// System.Void Unity.Entities.ComponentSystemBase.RequireForUpdate<Main.WorldBlackBoardTag>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<Main.GameOthersData>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<Main.GlobalConfigData>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<Main.HybridEventData>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<Main.PlayerData>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<Main.PrefabMapData>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<Main.WorldBlackBoardTag>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadOnly<Unity.Transforms.LocalTransform>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Main.DamageInfo>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Main.EnviromentData>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Main.GameCameraSizeData>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Main.GameEvent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Main.GameOthersData>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Main.GameRandomData>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Main.GameSetUpData>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Main.GlobalConfigData>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Main.HybridEventData>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Main.MapRefreshData>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Main.PlayerData>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Main.PrefabMapData>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Main.RenderingEntityTag>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Main.WorldBlackBoardTag>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Unity.Transforms.LocalTransform>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<Unity.Transforms.Parent>()
		// Unity.Entities.ComponentType Unity.Entities.ComponentType.ReadWrite<object>()
		// Unity.Entities.DynamicBuffer<Main.Buff> Unity.Entities.EntityDataAccess.GetBuffer<Main.Buff>(Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Main.BuffOld> Unity.Entities.EntityDataAccess.GetBuffer<Main.BuffOld>(Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Main.DamageInfo> Unity.Entities.EntityDataAccess.GetBuffer<Main.DamageInfo>(Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Main.GameEvent> Unity.Entities.EntityDataAccess.GetBuffer<Main.GameEvent>(Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Main.Skill> Unity.Entities.EntityDataAccess.GetBuffer<Main.Skill>(Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Main.State> Unity.Entities.EntityDataAccess.GetBuffer<Main.State>(Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Main.Trigger> Unity.Entities.EntityDataAccess.GetBuffer<Main.Trigger>(Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Unity.Entities.LinkedEntityGroup> Unity.Entities.EntityDataAccess.GetBuffer<Unity.Entities.LinkedEntityGroup>(Unity.Entities.Entity,bool)
		// Main.ChaStats Unity.Entities.EntityDataAccess.GetComponentData<Main.ChaStats>(Unity.Entities.Entity)
		// Main.DropsData Unity.Entities.EntityDataAccess.GetComponentData<Main.DropsData>(Unity.Entities.Entity)
		// Main.EnemyData Unity.Entities.EntityDataAccess.GetComponentData<Main.EnemyData>(Unity.Entities.Entity)
		// Main.EntityGroupData Unity.Entities.EntityDataAccess.GetComponentData<Main.EntityGroupData>(Unity.Entities.Entity)
		// Main.EnviromentData Unity.Entities.EntityDataAccess.GetComponentData<Main.EnviromentData>(Unity.Entities.Entity)
		// Main.GameOthersData Unity.Entities.EntityDataAccess.GetComponentData<Main.GameOthersData>(Unity.Entities.Entity)
		// Main.GameSetUpData Unity.Entities.EntityDataAccess.GetComponentData<Main.GameSetUpData>(Unity.Entities.Entity)
		// Main.GameTimeData Unity.Entities.EntityDataAccess.GetComponentData<Main.GameTimeData>(Unity.Entities.Entity)
		// Main.GlobalConfigData Unity.Entities.EntityDataAccess.GetComponentData<Main.GlobalConfigData>(Unity.Entities.Entity)
		// Main.IntroGuideItemData Unity.Entities.EntityDataAccess.GetComponentData<Main.IntroGuideItemData>(Unity.Entities.Entity)
		// Main.PlayerData Unity.Entities.EntityDataAccess.GetComponentData<Main.PlayerData>(Unity.Entities.Entity)
		// Main.PrefabMapData Unity.Entities.EntityDataAccess.GetComponentData<Main.PrefabMapData>(Unity.Entities.Entity)
		// Main.SpineHybridData Unity.Entities.EntityDataAccess.GetComponentData<Main.SpineHybridData>(Unity.Entities.Entity)
		// Main.StateMachine Unity.Entities.EntityDataAccess.GetComponentData<Main.StateMachine>(Unity.Entities.Entity)
		// Main.WeaponData Unity.Entities.EntityDataAccess.GetComponentData<Main.WeaponData>(Unity.Entities.Entity)
		// ProjectDawn.Navigation.AgentBody Unity.Entities.EntityDataAccess.GetComponentData<ProjectDawn.Navigation.AgentBody>(Unity.Entities.Entity)
		// ProjectDawn.Navigation.AgentLocomotion Unity.Entities.EntityDataAccess.GetComponentData<ProjectDawn.Navigation.AgentLocomotion>(Unity.Entities.Entity)
		// ProjectDawn.Navigation.AgentShape Unity.Entities.EntityDataAccess.GetComponentData<ProjectDawn.Navigation.AgentShape>(Unity.Entities.Entity)
		// Unity.Physics.PhysicsMass Unity.Entities.EntityDataAccess.GetComponentData<Unity.Physics.PhysicsMass>(Unity.Entities.Entity)
		// Unity.Transforms.LocalTransform Unity.Entities.EntityDataAccess.GetComponentData<Unity.Transforms.LocalTransform>(Unity.Entities.Entity)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Main.ChaStats>(Unity.Entities.Entity,Main.ChaStats,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Main.DropsData>(Unity.Entities.Entity,Main.DropsData,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Main.EntityGroupData>(Unity.Entities.Entity,Main.EntityGroupData,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Main.EnviromentData>(Unity.Entities.Entity,Main.EnviromentData,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Main.GameCameraSizeData>(Unity.Entities.Entity,Main.GameCameraSizeData,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Main.GameOthersData>(Unity.Entities.Entity,Main.GameOthersData,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Main.GameRandomData>(Unity.Entities.Entity,Main.GameRandomData,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Main.GameSetUpData>(Unity.Entities.Entity,Main.GameSetUpData,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Main.GameTimeData>(Unity.Entities.Entity,Main.GameTimeData,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Main.GlobalConfigData>(Unity.Entities.Entity,Main.GlobalConfigData,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Main.MapRefreshData>(Unity.Entities.Entity,Main.MapRefreshData,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Main.PlayerData>(Unity.Entities.Entity,Main.PlayerData,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Main.PushColliderData>(Unity.Entities.Entity,Main.PushColliderData,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Main.SpineHybridData>(Unity.Entities.Entity,Main.SpineHybridData,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Main.StateMachine>(Unity.Entities.Entity,Main.StateMachine,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Main.WeaponData>(Unity.Entities.Entity,Main.WeaponData,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<ProjectDawn.Navigation.AgentBody>(Unity.Entities.Entity,ProjectDawn.Navigation.AgentBody,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<ProjectDawn.Navigation.AgentLocomotion>(Unity.Entities.Entity,ProjectDawn.Navigation.AgentLocomotion,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<ProjectDawn.Navigation.AgentShape>(Unity.Entities.Entity,ProjectDawn.Navigation.AgentShape,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Unity.Physics.PhysicsMass>(Unity.Entities.Entity,Unity.Physics.PhysicsMass,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Unity.Transforms.LocalTransform>(Unity.Entities.Entity,Unity.Transforms.LocalTransform,Unity.Entities.SystemHandle&)
		// System.Void Unity.Entities.EntityDataAccess.SetComponentData<Unity.Transforms.Parent>(Unity.Entities.Entity,Unity.Transforms.Parent,Unity.Entities.SystemHandle&)
		// object Unity.Entities.EntityDataAccessManagedComponentExtensions.GetComponentObject<object>(Unity.Entities.EntityDataAccess&,Unity.Entities.Entity,Unity.Entities.ComponentType)
		// Unity.Entities.DynamicBuffer<Main.DamageInfo> Unity.Entities.EntityManager.AddBuffer<Main.DamageInfo>(Unity.Entities.Entity)
		// Unity.Entities.DynamicBuffer<Main.GameEvent> Unity.Entities.EntityManager.AddBuffer<Main.GameEvent>(Unity.Entities.Entity)
		// bool Unity.Entities.EntityManager.AddComponent<Main.DamageInfo>(Unity.Entities.Entity)
		// bool Unity.Entities.EntityManager.AddComponent<Main.GameEvent>(Unity.Entities.Entity)
		// bool Unity.Entities.EntityManager.AddComponent<Main.WorldBlackBoardTag>(Unity.Entities.Entity)
		// bool Unity.Entities.EntityManager.AddComponentData<Main.EnviromentData>(Unity.Entities.Entity,Main.EnviromentData)
		// bool Unity.Entities.EntityManager.AddComponentData<Main.GameCameraSizeData>(Unity.Entities.Entity,Main.GameCameraSizeData)
		// bool Unity.Entities.EntityManager.AddComponentData<Main.GameOthersData>(Unity.Entities.Entity,Main.GameOthersData)
		// bool Unity.Entities.EntityManager.AddComponentData<Main.GameRandomData>(Unity.Entities.Entity,Main.GameRandomData)
		// bool Unity.Entities.EntityManager.AddComponentData<Main.GameSetUpData>(Unity.Entities.Entity,Main.GameSetUpData)
		// bool Unity.Entities.EntityManager.AddComponentData<Main.MapRefreshData>(Unity.Entities.Entity,Main.MapRefreshData)
		// bool Unity.Entities.EntityManager.AddComponentData<Unity.Transforms.Parent>(Unity.Entities.Entity,Unity.Transforms.Parent)
		// System.Void Unity.Entities.EntityManager.CompleteDependencyBeforeRO<Main.GameOthersData>()
		// System.Void Unity.Entities.EntityManager.CompleteDependencyBeforeRO<Main.GlobalConfigData>()
		// System.Void Unity.Entities.EntityManager.CompleteDependencyBeforeRO<Main.PrefabMapData>()
		// Unity.Entities.Entity Unity.Entities.EntityManager.CreateSingleton<Main.GlobalConfigData>(Main.GlobalConfigData,Unity.Collections.FixedString64Bytes)
		// Unity.Entities.Entity Unity.Entities.EntityManager.CreateSingletonEntityInternal<Main.GlobalConfigData>(Unity.Collections.FixedString64Bytes)
		// Unity.Entities.DynamicBuffer<Main.Buff> Unity.Entities.EntityManager.GetBuffer<Main.Buff>(Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Main.BuffOld> Unity.Entities.EntityManager.GetBuffer<Main.BuffOld>(Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Main.DamageInfo> Unity.Entities.EntityManager.GetBuffer<Main.DamageInfo>(Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Main.GameEvent> Unity.Entities.EntityManager.GetBuffer<Main.GameEvent>(Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Main.Skill> Unity.Entities.EntityManager.GetBuffer<Main.Skill>(Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Main.State> Unity.Entities.EntityManager.GetBuffer<Main.State>(Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Main.Trigger> Unity.Entities.EntityManager.GetBuffer<Main.Trigger>(Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Unity.Entities.LinkedEntityGroup> Unity.Entities.EntityManager.GetBuffer<Unity.Entities.LinkedEntityGroup>(Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Main.Buff> Unity.Entities.EntityManager.GetBufferInternal<Main.Buff>(Unity.Entities.EntityDataAccess*,Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Main.BuffOld> Unity.Entities.EntityManager.GetBufferInternal<Main.BuffOld>(Unity.Entities.EntityDataAccess*,Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Main.DamageInfo> Unity.Entities.EntityManager.GetBufferInternal<Main.DamageInfo>(Unity.Entities.EntityDataAccess*,Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Main.GameEvent> Unity.Entities.EntityManager.GetBufferInternal<Main.GameEvent>(Unity.Entities.EntityDataAccess*,Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Main.Skill> Unity.Entities.EntityManager.GetBufferInternal<Main.Skill>(Unity.Entities.EntityDataAccess*,Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Main.State> Unity.Entities.EntityManager.GetBufferInternal<Main.State>(Unity.Entities.EntityDataAccess*,Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Main.Trigger> Unity.Entities.EntityManager.GetBufferInternal<Main.Trigger>(Unity.Entities.EntityDataAccess*,Unity.Entities.Entity,bool)
		// Unity.Entities.DynamicBuffer<Unity.Entities.LinkedEntityGroup> Unity.Entities.EntityManager.GetBufferInternal<Unity.Entities.LinkedEntityGroup>(Unity.Entities.EntityDataAccess*,Unity.Entities.Entity,bool)
		// Main.ChaStats Unity.Entities.EntityManager.GetComponentData<Main.ChaStats>(Unity.Entities.Entity)
		// Main.DropsData Unity.Entities.EntityManager.GetComponentData<Main.DropsData>(Unity.Entities.Entity)
		// Main.EnemyData Unity.Entities.EntityManager.GetComponentData<Main.EnemyData>(Unity.Entities.Entity)
		// Main.EntityGroupData Unity.Entities.EntityManager.GetComponentData<Main.EntityGroupData>(Unity.Entities.Entity)
		// Main.EnviromentData Unity.Entities.EntityManager.GetComponentData<Main.EnviromentData>(Unity.Entities.Entity)
		// Main.GameOthersData Unity.Entities.EntityManager.GetComponentData<Main.GameOthersData>(Unity.Entities.Entity)
		// Main.GameSetUpData Unity.Entities.EntityManager.GetComponentData<Main.GameSetUpData>(Unity.Entities.Entity)
		// Main.GameTimeData Unity.Entities.EntityManager.GetComponentData<Main.GameTimeData>(Unity.Entities.Entity)
		// Main.GlobalConfigData Unity.Entities.EntityManager.GetComponentData<Main.GlobalConfigData>(Unity.Entities.Entity)
		// Main.IntroGuideItemData Unity.Entities.EntityManager.GetComponentData<Main.IntroGuideItemData>(Unity.Entities.Entity)
		// Main.PlayerData Unity.Entities.EntityManager.GetComponentData<Main.PlayerData>(Unity.Entities.Entity)
		// Main.PrefabMapData Unity.Entities.EntityManager.GetComponentData<Main.PrefabMapData>(Unity.Entities.Entity)
		// Main.SpineHybridData Unity.Entities.EntityManager.GetComponentData<Main.SpineHybridData>(Unity.Entities.Entity)
		// Main.StateMachine Unity.Entities.EntityManager.GetComponentData<Main.StateMachine>(Unity.Entities.Entity)
		// Main.WeaponData Unity.Entities.EntityManager.GetComponentData<Main.WeaponData>(Unity.Entities.Entity)
		// ProjectDawn.Navigation.AgentBody Unity.Entities.EntityManager.GetComponentData<ProjectDawn.Navigation.AgentBody>(Unity.Entities.Entity)
		// ProjectDawn.Navigation.AgentLocomotion Unity.Entities.EntityManager.GetComponentData<ProjectDawn.Navigation.AgentLocomotion>(Unity.Entities.Entity)
		// ProjectDawn.Navigation.AgentShape Unity.Entities.EntityManager.GetComponentData<ProjectDawn.Navigation.AgentShape>(Unity.Entities.Entity)
		// Unity.Physics.PhysicsMass Unity.Entities.EntityManager.GetComponentData<Unity.Physics.PhysicsMass>(Unity.Entities.Entity)
		// Unity.Transforms.LocalTransform Unity.Entities.EntityManager.GetComponentData<Unity.Transforms.LocalTransform>(Unity.Entities.Entity)
		// Unity.Entities.ComponentLookup<Main.GlobalConfigData> Unity.Entities.EntityManager.GetComponentLookup<Main.GlobalConfigData>(Unity.Entities.TypeIndex,bool)
		// Unity.Entities.ComponentLookup<Main.GlobalConfigData> Unity.Entities.EntityManager.GetComponentLookup<Main.GlobalConfigData>(bool)
		// Unity.Entities.ComponentLookup<Main.PrefabMapData> Unity.Entities.EntityManager.GetComponentLookup<Main.PrefabMapData>(Unity.Entities.TypeIndex,bool)
		// Unity.Entities.ComponentLookup<Main.PrefabMapData> Unity.Entities.EntityManager.GetComponentLookup<Main.PrefabMapData>(bool)
		// Unity.Entities.ComponentLookup<Unity.Transforms.LocalTransform> Unity.Entities.EntityManager.GetComponentLookup<Unity.Transforms.LocalTransform>(Unity.Entities.TypeIndex,bool)
		// Unity.Entities.ComponentLookup<Unity.Transforms.LocalTransform> Unity.Entities.EntityManager.GetComponentLookup<Unity.Transforms.LocalTransform>(bool)
		// object Unity.Entities.EntityManager.GetComponentObject<object>(Unity.Entities.Entity)
		// Unity.Entities.ComponentTypeHandle<Main.GameOthersData> Unity.Entities.EntityManager.GetComponentTypeHandle<Main.GameOthersData>(bool)
		// Unity.Entities.ComponentTypeHandle<Main.HybridEventData> Unity.Entities.EntityManager.GetComponentTypeHandle<Main.HybridEventData>(bool)
		// bool Unity.Entities.EntityManager.HasComponent<Main.RenderingEntityTag>(Unity.Entities.Entity)
		// bool Unity.Entities.EntityManager.HasComponent<object>(Unity.Entities.Entity)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Main.ChaStats>(Unity.Entities.Entity,Main.ChaStats)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Main.DropsData>(Unity.Entities.Entity,Main.DropsData)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Main.EntityGroupData>(Unity.Entities.Entity,Main.EntityGroupData)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Main.EnviromentData>(Unity.Entities.Entity,Main.EnviromentData)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Main.GameCameraSizeData>(Unity.Entities.Entity,Main.GameCameraSizeData)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Main.GameOthersData>(Unity.Entities.Entity,Main.GameOthersData)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Main.GameRandomData>(Unity.Entities.Entity,Main.GameRandomData)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Main.GameSetUpData>(Unity.Entities.Entity,Main.GameSetUpData)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Main.GameTimeData>(Unity.Entities.Entity,Main.GameTimeData)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Main.GlobalConfigData>(Unity.Entities.Entity,Main.GlobalConfigData)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Main.MapRefreshData>(Unity.Entities.Entity,Main.MapRefreshData)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Main.PlayerData>(Unity.Entities.Entity,Main.PlayerData)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Main.PushColliderData>(Unity.Entities.Entity,Main.PushColliderData)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Main.SpineHybridData>(Unity.Entities.Entity,Main.SpineHybridData)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Main.StateMachine>(Unity.Entities.Entity,Main.StateMachine)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Main.WeaponData>(Unity.Entities.Entity,Main.WeaponData)
		// System.Void Unity.Entities.EntityManager.SetComponentData<ProjectDawn.Navigation.AgentBody>(Unity.Entities.Entity,ProjectDawn.Navigation.AgentBody)
		// System.Void Unity.Entities.EntityManager.SetComponentData<ProjectDawn.Navigation.AgentLocomotion>(Unity.Entities.Entity,ProjectDawn.Navigation.AgentLocomotion)
		// System.Void Unity.Entities.EntityManager.SetComponentData<ProjectDawn.Navigation.AgentShape>(Unity.Entities.Entity,ProjectDawn.Navigation.AgentShape)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Unity.Physics.PhysicsMass>(Unity.Entities.Entity,Unity.Physics.PhysicsMass)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Unity.Transforms.LocalTransform>(Unity.Entities.Entity,Unity.Transforms.LocalTransform)
		// System.Void Unity.Entities.EntityManager.SetComponentData<Unity.Transforms.Parent>(Unity.Entities.Entity,Unity.Transforms.Parent)
		// Unity.Collections.NativeArray<Main.ChaStats> Unity.Entities.EntityQuery.ToComponentDataArray<Main.ChaStats>(Unity.Collections.AllocatorManager.AllocatorHandle)
		// Unity.Collections.NativeArray<Main.PlayerData> Unity.Entities.EntityQuery.ToComponentDataArray<Main.PlayerData>(Unity.Collections.AllocatorManager.AllocatorHandle)
		// Unity.Collections.NativeArray<Unity.Transforms.LocalTransform> Unity.Entities.EntityQuery.ToComponentDataArray<Unity.Transforms.LocalTransform>(Unity.Collections.AllocatorManager.AllocatorHandle)
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAll<Main.BossTag>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAll<Main.GameOthersData>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAll<Main.PlayerData>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAll<Main.PrefabMapData,Main.GlobalConfigData>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAll<Main.PrefabMapData>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAll<Main.WorldBlackBoardTag>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAll<ProjectDawn.Navigation.CrowdSurface>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAll<Unity.Entities.LinkedEntityGroup>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithAllRW<Main.HybridEventData>()
		// Unity.Entities.EntityQueryBuilder Unity.Entities.EntityQueryBuilder.WithNone<Main.WorldBlackBoardTag>()
		// Unity.Collections.NativeArray<Main.ChaStats> Unity.Entities.EntityQueryImpl.ToComponentDataArray<Main.ChaStats>(Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Entities.EntityQuery)
		// Unity.Collections.NativeArray<Main.PlayerData> Unity.Entities.EntityQueryImpl.ToComponentDataArray<Main.PlayerData>(Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Entities.EntityQuery)
		// Unity.Collections.NativeArray<Unity.Transforms.LocalTransform> Unity.Entities.EntityQueryImpl.ToComponentDataArray<Unity.Transforms.LocalTransform>(Unity.Collections.AllocatorManager.AllocatorHandle,Unity.Entities.EntityQuery)
		// Main.GlobalConfigData Unity.Entities.Internal.InternalCompilerInterface.GetComponentAfterCompletingDependency<Main.GlobalConfigData>(Unity.Entities.ComponentLookup<Main.GlobalConfigData>&,Unity.Entities.SystemState&,Unity.Entities.Entity)
		// Main.PrefabMapData Unity.Entities.Internal.InternalCompilerInterface.GetComponentAfterCompletingDependency<Main.PrefabMapData>(Unity.Entities.ComponentLookup<Main.PrefabMapData>&,Unity.Entities.SystemState&,Unity.Entities.Entity)
		// Main.HybridEventData Unity.Entities.Internal.InternalCompilerInterface.GetComponentData<Main.HybridEventData>(Unity.Entities.EntityManager,Unity.Entities.Entity,Unity.Entities.TypeIndex,Main.HybridEventData&)
		// Unity.Entities.ComponentLookup<Unity.Transforms.LocalTransform> Unity.Entities.Internal.InternalCompilerInterface.GetComponentLookup<Unity.Transforms.LocalTransform>(Unity.Entities.ComponentLookup<Unity.Transforms.LocalTransform>&,Unity.Entities.SystemState&)
		// System.IntPtr Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetChunkNativeArrayReadOnlyIntPtrWithoutChecks<Main.GameOthersData>(Unity.Entities.ArchetypeChunk&,Unity.Entities.ComponentTypeHandle<Main.GameOthersData>&)
		// Unity.Entities.Entity Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetCopyOfNativeArrayPtrElement<Unity.Entities.Entity>(System.IntPtr,int)
		// Unity.Entities.Internal.InternalCompilerInterface.UncheckedRefRO<Main.GameOthersData> Unity.Entities.Internal.InternalCompilerInterface.UnsafeGetUncheckedRefRO<Main.GameOthersData>(System.IntPtr,int)
		// System.Void Unity.Entities.Internal.InternalCompilerInterface.WriteComponentData<Main.HybridEventData>(Unity.Entities.EntityManager,Unity.Entities.Entity,Unity.Entities.TypeIndex,Main.HybridEventData&,Main.HybridEventData&)
		// Unity.Entities.ComponentLookup<Main.GlobalConfigData> Unity.Entities.SystemState.GetComponentLookup<Main.GlobalConfigData>(bool)
		// Unity.Entities.ComponentLookup<Main.PrefabMapData> Unity.Entities.SystemState.GetComponentLookup<Main.PrefabMapData>(bool)
		// Unity.Entities.ComponentLookup<Unity.Transforms.LocalTransform> Unity.Entities.SystemState.GetComponentLookup<Unity.Transforms.LocalTransform>(bool)
		// Unity.Entities.ComponentTypeHandle<Main.GameOthersData> Unity.Entities.SystemState.GetComponentTypeHandle<Main.GameOthersData>(bool)
		// Unity.Entities.ComponentTypeHandle<Main.HybridEventData> Unity.Entities.SystemState.GetComponentTypeHandle<Main.HybridEventData>(bool)
		// System.Void Unity.Entities.SystemState.RequireForUpdate<Main.HybridEventData>()
		// System.Void Unity.Entities.SystemState.RequireForUpdate<Main.PlayerData>()
		// System.Void Unity.Entities.SystemState.RequireForUpdate<Main.WorldBlackBoardTag>()
		// Unity.Entities.SystemTypeIndex Unity.Entities.TypeManager.GetSystemTypeIndex<object>()
		// Unity.Entities.SystemTypeIndex Unity.Entities.TypeManager.GetSystemTypeIndexNoThrow<object>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.BossTag>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.Buff>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.BuffOld>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.ChaStats>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.DamageInfo>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.DropsData>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.EnemyData>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.EntityGroupData>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.EnviromentData>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.GameCameraSizeData>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.GameEvent>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.GameOthersData>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.GameRandomData>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.GameSetUpData>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.GameTimeData>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.GlobalConfigData>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.HybridEventData>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.IntroGuideItemData>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.MapRefreshData>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.PlayerData>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.PrefabMapData>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.PushColliderData>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.RenderingEntityTag>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.Skill>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.SpineHybridData>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.State>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.StateMachine>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.Trigger>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.WeaponData>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Main.WorldBlackBoardTag>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<ProjectDawn.Navigation.AgentBody>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<ProjectDawn.Navigation.AgentLocomotion>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<ProjectDawn.Navigation.AgentShape>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<ProjectDawn.Navigation.CrowdSurface>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Unity.Entities.LinkedEntityGroup>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Unity.Physics.PhysicsMass>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Unity.Transforms.LocalTransform>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<Unity.Transforms.Parent>()
		// Unity.Entities.TypeIndex Unity.Entities.TypeManager.GetTypeIndex<object>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.BossTag>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.Buff>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.BuffOld>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.ChaStats>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.DamageInfo>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.DropsData>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.EnemyData>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.EntityGroupData>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.EnviromentData>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.GameCameraSizeData>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.GameEvent>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.GameOthersData>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.GameRandomData>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.GameSetUpData>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.GameTimeData>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.GlobalConfigData>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.HybridEventData>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.IntroGuideItemData>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.MapRefreshData>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.PlayerData>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.PrefabMapData>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.PushColliderData>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.RenderingEntityTag>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.Skill>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.SpineHybridData>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.State>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.StateMachine>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.Trigger>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.WeaponData>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Main.WorldBlackBoardTag>()
		// System.Void Unity.Entities.TypeManager.ManagedException<ProjectDawn.Navigation.AgentBody>()
		// System.Void Unity.Entities.TypeManager.ManagedException<ProjectDawn.Navigation.AgentLocomotion>()
		// System.Void Unity.Entities.TypeManager.ManagedException<ProjectDawn.Navigation.AgentShape>()
		// System.Void Unity.Entities.TypeManager.ManagedException<ProjectDawn.Navigation.CrowdSurface>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Unity.Entities.LinkedEntityGroup>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Unity.Physics.PhysicsMass>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Unity.Transforms.LocalTransform>()
		// System.Void Unity.Entities.TypeManager.ManagedException<Unity.Transforms.Parent>()
		// System.Void Unity.Entities.TypeManager.ManagedException<object>()
		// object Unity.Entities.World.GetOrCreateSystemManaged<object>()
		// object UnityEngine.AndroidJNIHelper.ConvertFromJNIArray<object>(System.IntPtr)
		// System.IntPtr UnityEngine.AndroidJNIHelper.GetMethodID<object>(System.IntPtr,string,object[],bool)
		// object UnityEngine.AndroidJavaObject.CallStatic<object>(string,object[])
		// object UnityEngine.AndroidJavaObject.FromJavaArray<object>(System.IntPtr)
		// object UnityEngine.AndroidJavaObject._CallStatic<object>(System.IntPtr,object[])
		// object UnityEngine.AndroidJavaObject._CallStatic<object>(string,object[])
		// object UnityEngine.Component.GetComponent<object>()
		// object UnityEngine.Component.GetComponentInParent<object>()
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.GameObject.GetComponent<object>()
		// object UnityEngine.GameObject.GetComponentInChildren<object>()
		// object UnityEngine.GameObject.GetComponentInChildren<object>(bool)
		// System.Void UnityEngine.GameObject.GetComponentsInChildren<object>(bool,System.Collections.Generic.List<object>)
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>()
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>(bool)
		// System.Void UnityEngine.GameObject.GetComponentsInParent<object>(bool,System.Collections.Generic.List<object>)
		// bool UnityEngine.GameObject.TryGetComponent<object>(object&)
		// object UnityEngine.JsonUtility.FromJson<object>(string)
		// object UnityEngine.Object.FindObjectOfType<object>()
		// object[] UnityEngine.Object.FindObjectsOfType<object>()
		// object UnityEngine.Object.Instantiate<object>(object)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform,bool)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
		// object[] UnityEngine.Resources.ConvertObjects<object>(UnityEngine.Object[])
		// UnityEngine.ResourceRequest UnityEngine.Resources.LoadAsync<object>(string)
		// System.Void UnityEngine.UI.LayoutGroup.SetProperty<float>(float&,float)
		// System.Void UnityEngine.UI.LayoutGroup.SetProperty<int>(int&,int)
		// object UnityEngine._AndroidJNIHelper.ConvertFromJNIArray<object>(System.IntPtr)
		// System.IntPtr UnityEngine._AndroidJNIHelper.GetMethodID<object>(System.IntPtr,string,object[],bool)
		// string UnityEngine._AndroidJNIHelper.GetSignature<object>(object[])
		// YooAsset.AssetHandle YooAsset.ResourcePackage.LoadAssetAsync<object>(string,uint)
		// YooAsset.AssetHandle YooAsset.ResourcePackage.LoadAssetSync<object>(string)
	}
}