namespace BankService.Domain.Interfaces;

public interface IMenuStrategy
{
    public abstract void ShowMenu();
    public abstract void HandleInput(int choice);
}