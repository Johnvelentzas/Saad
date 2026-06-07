
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Models;
using Models.Messages;
using Models.Production;
using Producion_Line_Manager.ViewModels;

namespace Producion_Line_Manager.Views.DetailsViews
{
    public partial class BaseEntityViewModel : BaseViewModel
    {
        [ObservableProperty]
        private IEntity? _item;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotDraft))]
        private bool _isDraft = false;

        public bool IsNotDraft => !IsDraft;

        [ObservableProperty]
        private int _Id = 0;

        [ObservableProperty]
        private DateTime _createdDate;
        [ObservableProperty]
        private string _comments = String.Empty;

        private CancellationTokenSource? _debounceTokenSource;

        partial void OnCommentsChanged(string value)
        {
            if (Item is Orders order)
            {
                order.Comments = value;
                TriggerDebouncedSave();
            }
        }
        protected void TriggerDebouncedSave()
        {
            // 1. Cancel the previous timer because the user is still interacting
            _debounceTokenSource?.Cancel();

            // 2. Create a new token for this specific pause
            _debounceTokenSource = new CancellationTokenSource();
            var token = _debounceTokenSource.Token;

            // 3. Start the background timer
            Task.Run(async () =>
            {
                try
                {
                    // Wait for exactly 2 seconds (2000 milliseconds)
                    await Task.Delay(2000, token);

                    // If the timer finishes and wasn't cancelled by new input...
                    if (!token.IsCancellationRequested)
                    {
                        // Broadcast the signal! 
                        // Your TabListViewModel will catch this and run the actual API save.
                        WeakReferenceMessenger.Default.Send(new SaveDraftMessage());
                    }
                }
                catch (TaskCanceledException)
                {
                    // Task.Delay throws this when we call .Cancel()
                    // This is expected behavior when the user keeps typing. Just swallow it.
                }
            });
        }

        protected void TriggerImmediateSave()
        {
            // Cancel any pending debounced save since we're doing an immediate save now
            _debounceTokenSource?.Cancel();
            // Broadcast the signal immediately
            WeakReferenceMessenger.Default.Send(new SaveDraftMessage());
        }


        public void LoadEntity(IEntity entity)
        {
            Item = entity;
            IsDraft = entity.IsDraft;
            Id = entity.Id;
            Comments = entity.Comments ?? String.Empty;
        }

        public virtual void SaveEntity()
        {
            if (Item == null) { return; }
            Item.Id = Id;
            Item.IsDraft = IsDraft;
            Item.Comments = Comments;
        }
    }
}
