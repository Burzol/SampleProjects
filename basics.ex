# just some simple things in  elixir

defmodule T do
  def start do
    the_program()
  end

  def the_program do
    name = IO.gets("Hello, what is your name?") |> String.trim()
    IO.puts("Hello #{name}.")
    IO.puts("")
    keepasking_question()
  end

  def keepasking_question do
    answer = IO.gets("Would you like to follow the White Rabbit? (Y/N)") |> String.trim()

    case answer do
      "Y" ->
        next_step()

      "y" ->
        next_step()

      "N" ->
        IO.puts("Thanks for playing!")

      "n" ->
        IO.puts("Thanks for playing!")

      _ ->
        keepasking_question()
    end
  end

  def next_step do
    IO.puts("A fun fact, today is #{DateTime.utc_now()}")
    keepasking_question2()
  end

  def keepasking_question2 do
    IO.puts(" ")

    heroes = %{
      "Iron Man" => "Tony Stark",
      "Captain America" => "Steve Rogers",
      "Hulk" => "Bruce Banner",
      "Captain Marvel" => "Carol Danvers",
      "Black Widow" => "Natasha Romanoff",
      "Black Panther" => "T`Challa",
      "Ant-Man" => "Scott Lang",
      "Thor" => "God Of Thunder",
      "Spider-Man" => "Peter Parker",
      "Winter Soldier" => "Bucky Barnes",
      "Falcon" => "Sam Wilson",
      "Venom" => "that cosmic Symbiote"
    }

    answer2 = IO.gets("Who is your favorite Avenger?") |> String.trim()
    # case 3 do
    #  1 -> IO.puts("ent 1")
    #    2 -> IO.puts("ent 2")
    #    _ -> IO.puts("def")
    #  end
    case answer2 do
      "Batman" ->
        IO.puts("The Dark Knight is not an Avenger!")
        keepasking_question2()

      "Superman" ->
        IO.puts("That's a plane? That's a bird? No! It's a son of Krytpton, not an Avenger!")
        keepasking_question2()

      "Wonder Woman" ->
        IO.puts("Diana of Amazons is cool, but she is not an Avenger.")
        keepasking_question2()

      "Shazam" ->
        IO.puts("Billy Batson is too young be an Avenger!")

      "exit" ->
        IO.puts("Okay, bye!")

      _ ->
        IO.puts("#{answer2}? So that means that #{heroes[answer2]} is your favorite person, huh?")
        keepasking_question2()
    end
  end
end
