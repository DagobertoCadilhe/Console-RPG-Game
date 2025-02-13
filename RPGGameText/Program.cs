using System;

namespace Program;

class Program
{
    static void Main(string[] args)
    {
        Player player = new Player(100, 0, 10, 2);
        Enemy enemy = new Enemy(50, 20, 10, 2);
        Battle battle = new Battle(player, enemy);

        battle.RunBattle();
    }
}

class Character
{
    public double Health { get; protected set; }
    public int Xp { get; private set; }
    public int AttackDamage { get; private set; }
    public int Defence { get; private set; }

    public Character(double health, int xp, int attackDamage, int defence)
    {
        if (health < 0) throw new ArgumentException("Health cannot be negative!");
        if (xp < 0) throw new ArgumentException("XP cannot be negative!");
        if (attackDamage < 0) throw new ArgumentException("Attack damage cannot be negative!");
        if (defence < 0) throw new ArgumentException("Defence cannot be negative!");

        Health = health;
        Xp = xp;
        AttackDamage = attackDamage;
        Defence = defence;
    }

    public void TakeDamage(double damage)
    {
        if (damage > 0)
        {
            Health -= (damage - Defence);
            if (Health < 0) Health = 0;
        }
    }

    public void GainExperience(int xpGiven)
    {
        if (xpGiven > 0)
        {
            Xp += xpGiven;
        }
    }

    public override string ToString()
    {
        return $"Health: {Health}, XP: {Xp}, Attack Damage: {AttackDamage}, Defence: {Defence}";
    }
}

class Player : Character
{
    public Player(double health, int xp, int attackDamage, int defence) : base(health, xp, attackDamage, defence) { }
    public readonly string[] playerOptions = { "Attack", "Heal", "Quit" };

    public void Heal(double amount)
    {
        if (amount > 0)
        {
            Health += amount;
            if (Health > 100) Health = 100; // Limite máximo de saúde
        }
    }
}

class Enemy : Character
{
    public Enemy(double health, int xp, int attackDamage, int defence) : base(health, xp, attackDamage, defence) { }
}

class Battle
{
    public Player Player { get; set; }
    public Enemy Enemy { get; set; }

    private bool _isRunning = true;
    private bool _playerTurn = true;

    public Battle(Player player, Enemy enemy)
    {
        Player = player;
        Enemy = enemy;
    }

    public void RunBattle()
    {
        while (_isRunning)
        {
            CheckBattleWinner();

            if (_playerTurn)
            {
                Console.WriteLine("What the player is going to do?");
                PrintOptions();
                var option = ReadOption();
                HandleOption(option);
            }
            else
            {
                Console.WriteLine("Enemy attacks!");
                EnemyDealDamage(Enemy, Player);
                _playerTurn = true;
            }
        }
    }

    private void PrintOptions()
    {
        Console.WriteLine("-----------------");
        for (int i = 0; i < Player.playerOptions.Length; i++)
        {
            Console.WriteLine($"{i + 1} - {Player.playerOptions[i]}");
        }
        Console.WriteLine("-----------------");
    }

    private int ReadOption()
    {
        int option = 0;
        while (!int.TryParse(Console.ReadLine(), out option))
        {
            Console.WriteLine("Invalid input. Please enter a number.");
        }
        return option - 1;
    }

    private void HandleOption(int option)
    {
        switch (option)
        {
            case 0:
                Console.WriteLine("Player attacks!");
                PlayerDealDamage(Player, Enemy);
                _playerTurn = false;
                break;

            case 1:
                Console.WriteLine("Player heals!");
                Player.Heal(10);
                _playerTurn = false;
                break;

            case 2:
                Console.WriteLine("Player abandons the fight!");
                _isRunning = false;
                break;
        }
    }

    private void CheckBattleWinner()
    {
        if (Player.Health <= 0)
        {
            Console.WriteLine("Enemy Wins!");
            _isRunning = false;
        }
        if (Enemy.Health <= 0)
        {
            Console.WriteLine("Player Wins!");
            Player.GainExperience(Enemy.Xp);
            Console.WriteLine($"Player gained {Enemy.Xp} XP!");
            _isRunning = false;
        }
    }

    public void PlayerDealDamage(Player player, Enemy enemy)
    {
        double damageDealt = player.AttackDamage - enemy.Defence;
        enemy.TakeDamage(damageDealt);
        Console.WriteLine($"Player hits enemy for {damageDealt} damage!");
    }

    public void EnemyDealDamage(Enemy enemy, Player player)
    {
        double damageDealt = enemy.AttackDamage - player.Defence;
        player.TakeDamage(damageDealt);
        Console.WriteLine($"Enemy hits player for {damageDealt} damage!");
    }
}