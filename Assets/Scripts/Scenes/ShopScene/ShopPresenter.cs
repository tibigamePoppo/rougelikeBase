using UnityEngine;

namespace Scenes.Shopscene
{
    public class ShopPresenter : MonoBehaviour
    {
        private ShopModel _model;
        private ShopView _view;
        void Start()
        {
            Init();
        }

        public void Init()
        {
            _model = new ShopModel();
            _view = GetComponent<ShopView>();
            _model.Init();
            _view.Init();
        }
    }
}