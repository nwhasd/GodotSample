using Godot;
using System;
using System.IO;

public class Main : Node
{
	[Export]
	public PackedScene Mob;

	private int _score;

	private Random _random = new Random();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode("Player").Connect("Hit", this, "GameOver");
		GetNode("StartTimer").Connect("timeout", this, "OnStartTimerTimeout");
		GetNode("ScoreTimer").Connect("timeout", this, "OnScoreTimerTimeout");
		GetNode("MobTimer").Connect("timeout", this, "OnMobTimerTimeout");
		GetNode("HUD").Connect("StartGame", this, "OnStartGame");
	}

	public void OnStartGame()
	{
		NewGame();
	}

	public void OnStartTimerTimeout()
	{
		GetNode<Timer>("ScoreTimer").Start();
		GetNode<Timer>("MobTimer").Start();
	}

	public void OnScoreTimerTimeout()
	{
		_score++;
		GetNode<HUD>("HUD").UpdateScore(_score);
	}

	public void OnMobTimerTimeout()
	{
		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.Offset = _random.Next();

		var mobInstace = (RigidBody2D)Mob.Instance();
		AddChild(mobInstace);

		float dir = mobSpawnLocation.Rotation + Mathf.Pi / 2;
		mobInstace.Position = mobSpawnLocation.Position;

		dir += RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		mobInstace.Rotation = dir;

		mobInstace.LinearVelocity = new Vector2(RandRange(150f, 250f), 0).Rotated(dir);
	}

	private float RandRange(float min, float max)
	{
		return (float)_random.NextDouble() * (max - min) + min;
	}

	public void GameOver()
	{
		GetNode<Timer>("MobTimer").Stop();
		GetNode<Timer>("ScoreTimer").Stop();
		GetNode<HUD>("HUD").ShowGameOver();

		GetTree().CallGroup("mobs", "queue_free");
		GetNode<AudioStreamPlayer>("Music").Stop();
		GetNode<AudioStreamPlayer>("DeathSound").Play();
	}

	public void NewGame()
	{
		_score = 0;
		var player = GetNode<Player>("Player");
		var startPositon = GetNode<Position2D>("StartPosition");
		player.Start(startPositon.Position);

		GetNode<Timer>("StartTimer").Start();

		var hud = GetNode<HUD>("HUD");
		hud.UpdateScore(_score);
		hud.ShowMessage("Get Ready!");

		GetNode<AudioStreamPlayer>("Music").Play();

		try
		{
			// 创建一个 StreamReader 的实例来读取文件 
			// using 语句也能关闭 StreamReader
			using (StreamReader sr = new StreamReader("c:/jamaica.txt"))
			{
				string line;
				
				// 从文件读取并显示行，直到文件的末尾 
				while ((line = sr.ReadLine()) != null)
				{
					GD.Print(line);
				}
			}

			using (StreamWriter sw = new StreamWriter("c:/names.txt"))
            {

                    sw.WriteLine("nihao");
            }
		}
		catch (Exception e)
		{
		}

		ImageTexture texture = load_external_png("C:/a.png");
		GetNode<Sprite>("Sprite").Texture = texture;
	}

	ImageTexture load_external_png(string filepath)
	{

		var f = new Godot.File();
		f.Open(filepath, Godot.File.ModeFlags.Read);
		var buffer = f.GetBuffer((int)f.GetLen());
		f.Close();
		var img = new Image();
		if (img.LoadPngFromBuffer(buffer) != 0)
		{
			GD.Print("Error, Load Image Failure");
			return null;
		}

		var texture = new ImageTexture();
		texture.CreateFromImage(img);
		return texture;
	}


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
