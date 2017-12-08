using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using System;
using System.Collections.Generic;

namespace Registrator.Module.Win.Controllers
{
    /// <summary>
    /// Контроллер создания нового объекта
    /// </summary>
    /// <remarks>Переопределяет список создаваемых типов в главном окне, удаляя все типы, 
    /// не соответствующие типу объектов представления.
    /// Для вложенных представлений список остается полным.</remarks>
    public class NewObjectController : WinNewObjectViewController
    {
        /// <inheritdoc/>
        protected override void OnActivated()
        {
            base.OnActivated();
            ShowNavigationItemController navigationController = Frame.GetController<ShowNavigationItemController>();
            navigationController.ShowNavigationItemAction.SelectedItemChanged += ShowNavigationItemAction_SelectedItemChanged;
            CollectCreatableItemTypes += MyController_CollectCreatableItemTypes;
        }

        /// <inheritdoc/>
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            ShowNavigationItemController navigationController = Frame.GetController<ShowNavigationItemController>();
            navigationController.ShowNavigationItemAction.SelectedItemChanged -= ShowNavigationItemAction_SelectedItemChanged;
            CollectCreatableItemTypes -= MyController_CollectCreatableItemTypes;
        }

        private void MyController_CollectCreatableItemTypes(object sender, CollectTypesEventArgs e)
        {
            CustomizeList(e.Types);
        }

        private void ShowNavigationItemAction_SelectedItemChanged(object sender, EventArgs e)
        {
            this.UpdateActionState();
        }

        /// <summary>
        /// Настраивает список создаваемых типов
        /// </summary>
        /// <param name="types">Список создаваемых типов</param>
        protected virtual void CustomizeList(ICollection<Type> types)
        {
            DevExpress.ExpressApp.Win.WinWindow winWindow = Frame as DevExpress.ExpressApp.Win.WinWindow;
            if (winWindow != null)
            {
                var view = winWindow.View;

                if (view != null && view.ObjectTypeInfo != null && view.ObjectTypeInfo.Type != null)
                {
                    List<Type> unusableTypes = new List<Type>();
                    foreach (Type type in types)
                    {
                        bool deletionRequired = type != view.ObjectTypeInfo.Type;
                        if (deletionRequired)
                            unusableTypes.Add(type);
                    }
                    foreach (Type type in unusableTypes)
                        types.Remove(type);
                }
            }
        }
    }
}
