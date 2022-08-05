
using Constructor.Details;

namespace UI.Models
{
    public interface IDetailViewer
    {
        void Show();
        void Hide();
        void SetDetail(Detail detail);
    }

    public interface IApplyDetailViewerToObj
    {
        void SetPreviewObj(FbxDetail fbxDetail);
    }
}
