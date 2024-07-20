using TTT.Formatting.Core;

namespace TTT.Formatting.Base;

public interface IView {
  void Render(FormatWriter writer);
}