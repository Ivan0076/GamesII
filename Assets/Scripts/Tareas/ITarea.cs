public interface ITarea
{
    string nombreTarea { get; }
    bool EstaCompletada { get; }
    bool PuedeIniciarse();
    void IniciarTarea();
    void CompletarTarea();
}