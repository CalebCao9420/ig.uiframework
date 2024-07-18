using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IG.Module.UI{
	[RequireComponent(typeof(TableView))]
	public class TableViewExtendBanner : MonoBehaviour {
		public TextMeshProUGUI PageStatus,tipsText; 
		[SerializeField]
		private Button _prev;
		[SerializeField]
		private Button _next;
		[Tooltip("点击prev next的Dotween动画速度")]
		[SerializeField]
		private float _moveSpeed = 10f;
		[Tooltip("点击prev next的Dotween的Ease")]
		[SerializeField]
		private Ease _ease = Ease.InOutQuad;
		private TableView _tableView;
		void Awake(){
			_tableView = this.GetComponent<TableView>();
			_tableView.onSetDataComplete = OnSetDatasComplete;
			_tableView.OnValueChanged.AddListener (OnTableViewViewChange);
			if (_next != null) {
				_next.onClick.AddListener (OnNextButton);
			}
			if (_prev != null) {
				_prev.onClick.AddListener (OnPrevButton);
			}
		}

		void OnSetDatasComplete(){
			CheckButtonStatus();
			ShowPageStatus ();
		}

		void OnTableViewViewChange(Vector2 offset){
			CheckButtonStatus ();
			ShowPageStatus ();
		}

		void ShowPageStatus(){
			int dataIndex = _tableView.StartCellDataIndex;
			if (dataIndex <= 0) {
				dataIndex = 0;
			}
			if(dataIndex >= _tableView.Data.Length-1) {
				dataIndex = _tableView.Data.Length-1;
			}
			PageStatus.text = ((dataIndex + 1) + "/" + _tableView.Data.Length);
			//tipsText.text = (tableView.datas[dataIndex] as MGuideTips).description.ToRichString();
		}

		void CheckButtonStatus(){
			if (!_tableView.Loop) {
				if (_tableView.StartCellDataIndex >= (_tableView.Data.Length - 1)) {
					if (_next != null) {
						_next.SetInteractable(false);
						//next.interactable = false;
						_next.gameObject.SetActive(_tableView.Data.Length>1);
					}
				} else {
					if (_next != null) {
						_next.SetInteractable(true);
						_next.gameObject.SetActive(_tableView.Data.Length>1);
						//next.interactable = true;
					}
				}
				if (_tableView.StartCellDataIndex <= 0) {
					if (_prev != null) {
						_prev.SetInteractable(false);
						_prev.gameObject.SetActive(_tableView.Data.Length>1);
						//prev.interactable = false;
					}
				} else {
					if (_prev != null) {
						_prev.SetInteractable(true);
						_prev.gameObject.SetActive(_tableView.Data.Length>1);
						//prev.interactable = true;
					}
				}
			}
		}

		/// <summary>
		/// 如果是垂直滚动则向下滚动步长,如果是水平滚动则向右是
		/// </summary>
		/// <param name="stepNum">Step number.</param>
		public void SetContentAnchorPos(float pos){
			if (_tableView.ScrollDirection == EHorzOrVert.Vertical) {
				_tableView.Content.anchoredPosition = new Vector2 (_tableView.Content.anchoredPosition.x, pos);
			} else {
				_tableView.Content.anchoredPosition = new Vector2 (pos, _tableView.Content.anchoredPosition.y);
			}
		}
			
		private Tweener _tweener;
		void OnPrevButton(){
			if (_tableView.InElasticing && (_tweener==null || !_tweener.IsPlaying())) {
				if (_tableView.ScrollDirection == EHorzOrVert.Vertical) {
					//暂时未用ToDo
//					tweener = DOTween.To (() => {
//						return tableView.content.anchoredPosition.y;
//					}, (x) => {
//						SetContentAnchorPos (x);
//					}, 0, tableView.content.anchoredPosition.y / moveSpeed).SetEase (ease).OnComplete (() => {
//						CheckButtonStatus ();
//					});
				} else {
					_tweener = DOTween.To (() => {
						return _tableView.Content.anchoredPosition.x;
					}, (x) => {
						SetContentAnchorPos (x);
					}, -1*(_tableView.Data.Length-2)*_tableView.StepSize,_tableView.StepSize/ _moveSpeed).SetEase (_ease).OnComplete (() => {
						CheckButtonStatus ();
					});
				}
			} else if((_tweener==null || !_tweener.IsPlaying()) && !_tableView.InAnimation){
				if (_tableView.ScrollDirection == EHorzOrVert.Vertical) {
					_tweener = DOTween.To (() => {
						return _tableView.Content.anchoredPosition.y;
					}, (x) => {
						SetContentAnchorPos(x);
					}, _tableView.Content.anchoredPosition.y-_tableView.StepSize, _tableView.StepSize/_moveSpeed).SetEase (_ease).OnComplete(()=>{

						CheckButtonStatus();
					});
				} else {
					_tweener = DOTween.To (() => {
						return _tableView.Content.anchoredPosition.x;
					}, (x) => {
						SetContentAnchorPos(x);
					}, _tableView.Content.anchoredPosition.x+_tableView.StepSize, _tableView.StepSize/_moveSpeed).SetEase (_ease).OnComplete(()=>{
						CheckButtonStatus();
					});
				}
			}
		}

		void OnNextButton(){
			if (_tableView.InElasticing && (_tweener==null || !_tweener.IsPlaying())) {
				if (_tableView.ScrollDirection == EHorzOrVert.Vertical) {
					//暂时未用ToDo
//					tweener = DOTween.To (() => {
//						return tableView.content.anchoredPosition.y;
//					}, (x) => {
//						SetContentAnchorPos (x);
//					}, -1*tableView.stepSize, tableView.stepSize-tableView.content.anchoredPosition.y / moveSpeed).SetEase (ease).OnComplete (() => {
//						CheckButtonStatus ();
//					});
				} else {
					_tweener = DOTween.To (() => {
						return _tableView.Content.anchoredPosition.x;
					}, (x) => {
						SetContentAnchorPos (x);
					}, -1*_tableView.StepSize,_tableView.StepSize / _moveSpeed).SetEase (_ease).OnComplete (() => {
						CheckButtonStatus ();
					});
				}
			} else if((_tweener==null || !_tweener.IsPlaying()) && !_tableView.InAnimation){
				if (_tableView.ScrollDirection == EHorzOrVert.Vertical) {
					_tweener = DOTween.To (() => {
						return _tableView.Content.anchoredPosition.y;
					}, (x) => {
						SetContentAnchorPos(x);
					}, _tableView.Content.anchoredPosition.y+_tableView.StepSize, _tableView.StepSize/_moveSpeed).SetEase (_ease).OnComplete(()=>{

						CheckButtonStatus();
					});
				} else {
					_tweener = DOTween.To (() => {
						return _tableView.Content.anchoredPosition.x;
					}, (x) => {
						SetContentAnchorPos(x);
					}, _tableView.Content.anchoredPosition.x-_tableView.StepSize, _tableView.StepSize/_moveSpeed).SetEase (_ease).OnComplete(()=>{
						CheckButtonStatus();
					});
				}
			}
		}	
	}
}
