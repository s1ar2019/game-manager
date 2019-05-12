Public Class Testing

    Private Class cPlayer
        Public Property ID As Integer
        Public Property UserType As Boolean
        Public Property Nickname As String
        Public Property Password As String
        Public Property Email As String
        Public Property XP As Integer
        Public Property Credit As Integer
        Public Property Games As List(Of cGame)
        Public Property PropertyNames = New String() {"ID", "UserType", "Nickname", "Password", "Email", "XP", "Credit", "Games"}
    End Class

    Private Class cGame
        Public Property Name As String
        Public Property Unlocked As Boolean
        Public Property Credit As Integer
        Public Property BestScore As Integer
        Public Property Trophies As List(Of cTrophy)
    End Class

    Private Class cTrophy
        Public Property Name As String
        Public Property Unlocked As Boolean
        Public Property Credit As Integer
    End Class

    Dim players As New List(Of cPlayer)
    Dim path As String = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), "UsersData.txt")

    Public Sub New()
        InitializeComponent()
        init_UI()
        read_file()
        show_info()
        save_file()
    End Sub

    Private Sub show_info()
        Dim grid As New DataGridView
        Me.Controls.Add(grid)
        'Grid properties
        With grid
            .Size = New Size(865, 700)
            .Location = New Point(2, 27)
            .AllowUserToDeleteRows = False
            .AllowUserToOrderColumns = False
            .AllowUserToResizeColumns = False
            .AllowUserToResizeRows = False
            .AllowUserToAddRows = False
            .ReadOnly = True
            .ScrollBars = ScrollBars.Vertical
        End With
        'Grid content
        If players.Count > 0 Then
            For Each prop In players(0).PropertyNames
                Dim column As New DataGridViewColumn
                column.HeaderText = prop
                column.Frozen = False
                Dim cell As DataGridViewCell = New DataGridViewTextBoxCell
                cell.Style.BackColor = Color.Cornsilk
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                column.CellTemplate = cell
                grid.Columns.Add(column)
            Next
            For Each player In players
                With player
                    Dim row = New String() {.ID, .UserType, .Nickname, .Password, .Email, .XP, .Credit, .Games.Count}
                    grid.Rows.Add(row)
                End With
            Next
        End If
    End Sub

    Private Sub init_UI()
        Me.IsMdiContainer = True
        Me.Size = New Size(1500, 900)
        Me.BackColor = Color.Cornsilk
        For Each ctl As Control In Me.Controls
            If TypeOf ctl Is MdiClient Then
                ctl.BackColor = Me.BackColor
            End If
        Next ctl
        Dim file, tom, ha As New ToolStripDropDownButton
        tom.Text = "tom"
        tom.DropDownItems.Add(ha)
        tom.DropDownDirection = ToolStripDropDownDirection.Right
        ha.Text = "ha"
        ha.DropDownDirection = ToolStripDropDownDirection.BelowRight
        With file
            .Text = "File"
            .ShowDropDownArrow = False
            .DropDownItems.Add(tom)
        End With
        Dim menuStrip As New MenuStrip
        With menuStrip
            .Dock = DockStyle.Top
            .Items.Add(file)
        End With
        Me.Controls.Add(menuStrip)

    End Sub

    Private Sub read_file()
        Dim text_in_file As String
        If System.IO.File.Exists(path) Then
            text_in_file = My.Computer.FileSystem.ReadAllText(path)
            If text_in_file <> "" Then
                fill_players(text_in_file)
            Else
                MsgBox("File is empty")
            End If
        Else
            MsgBox("Missing File")
        End If
    End Sub

    Private Sub fill_players(ByVal players_string As String)
        Dim players_array() = Split(players_string, vbCrLf)
        For i = 0 To players_array.GetUpperBound(0)
            If players_array(i) <> "" Then
                Dim new_player As New cPlayer
                Dim player_info() As String = Split(players_array(i), "#games:")
                Dim player_parts() As String = Split(player_info(0), ";")
                With new_player
                    .ID = player_parts(0)
                    .UserType = player_parts(1)
                    .Nickname = player_parts(2)
                    .Password = player_parts(3)
                    .Email = player_parts(4)
                    .XP = player_parts(5)
                    .Credit = player_parts(6)
                    .Games = fill_games(player_info(1))
                End With
                players.Add(new_player)
            End If
        Next
    End Sub

    Private Function fill_games(ByVal games_string As String) As List(Of cGame)
        Dim games As New List(Of cGame)
        Dim games_array() = Split(games_string, "#game:")
        For i = 1 To games_array.GetUpperBound(0)
            Dim new_game As New cGame
            Dim game_info() As String = Split(games_array(i), "#trophies:")
            Dim game_parts() As String = Split(game_info(0), ";")
            With new_game
                .Name = game_parts(0)
                .Unlocked = game_parts(1)
                .Credit = game_parts(2)
                .BestScore = game_parts(3)
                .Trophies = fill_trophies(game_info(1))
            End With
            games.Add(new_game)
        Next
        Return games
    End Function

    Private Function fill_trophies(ByVal trophies_string As String) As List(Of cTrophy)
        Dim trophies As New List(Of cTrophy)
        Dim trophies_array() = Split(trophies_string, "#trophy:")
        For i = 1 To trophies_array.GetUpperBound(0)
            Dim new_trophy As New cTrophy
            Dim trophy_parts() As String = Split(trophies_array(i), ";")
            With new_trophy
                .Name = trophy_parts(0)
                .Unlocked = trophy_parts(1)
                .Credit = trophy_parts(2)
            End With
            trophies.Add(new_trophy)
        Next
        Return trophies
    End Function

    Private Sub save_file()
        Dim player_string As String = ""
        For Each player In players
            With player
                player_string &= .ID & ";" & .UserType & ";" & .Nickname & ";" & .Password & ";" & .Email & ";" & .XP & ";" & .Credit & ";" & "#games:"
            End With
            For Each game In player.Games
                With game
                    player_string &= "#game:" & .Name & ";" & .Unlocked & ";" & .Credit & ";" & .BestScore & "#trophies:"
                End With
                For Each trophy In game.Trophies
                    With trophy
                        player_string &= "#trophy:" & .Name & ";" & .Unlocked & ";" & .Credit & ";"
                    End With
                Next
            Next
            player_string &= vbCrLf
        Next
        player_string.Trim(vbCrLf)
        My.Computer.FileSystem.WriteAllText(path, player_string, False)
    End Sub

End Class