   M S B A M L   `   `   `  ����B  >XEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:XEditorXEditor  5   	       XEditor.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:XEditor     5   	   -   5          � Title$  XEditor��5   	   $
��1080��6   $	��600��6&    J� 	AllowDrop .  � 	Resources 5	      )   #XEditor.EnumMatchToBooleanConverter   EnumConverter��%   &	          5
   
   6,   5      .��5      � 
�������.+�5   
   ]� $��0,0,0,24q�6   .+�5      o� -   5         o�Name$ DockMenu��5      $	��Top=�6%   .@�5      m� $��_File��6   .@�5      m� -   5        $
��_New��5       m� InputGestureText$ Ctrl + N��6D   m� 5      -   5        $��_Open��5      $ Ctrl + O��6F   m� 5      -   5         m�Name$ 
Menu_Close��5      $��_Close��63   6C   m� 5      -   5        $ 	Menu_Save��5      $��_Save��62   $ Ctrl + S��6Y   m� 5      -   5        $ Menu_SaveAs��5      $��_Save as��64   6F   m� 5      -   5        $ Menu_RecentFiles��5      $��Recent files��69   6Q   m� 5      -	   5        $
 Exit��5      $��_Exit��6-   $ Alt + F4��6T   5      m� 5      $
��Edit��6   .@�5      m� -
   5        $ Menu_LevelSettings��5      $��Level Settings��6;   {�.  6o   5      m� 5      $��Preferences��6   .@�5      m� -   5        $ Menu_SaveAsCompressed��5      $��Save as compressed��6>    m� IsCheckable . 6   m� 5      -   5        $ Menu_ShowGrid��5      $��	Show grid��66    . 6f   m� 5      -   5        $ Menu_DefaultLayers��5      $��Update default layers��6;   6Z   m� 5      -   5        $ Menu_ViewLayerMode��5      $��ViewLayerMode��6;   .@�5       m� -   5        $ Menu_ViewLayerMode_Solid��5    6    . $��Solid��6X   6g   m� 5!      -   5        $$ Menu_ViewLayerMode_CurrentOnly��5!   6    . $��Current only��6^   6t   m� 5"      -   5        $# Menu_ViewLayerMode_FadeOthers��5"   6    . $��Fade others��6]   6r   5#      5$      5%      � 5'      -   5         �Name$ EditorContainer��5'      $y�Visible=�6,    � ColumnDefinitions 5(      �� 5)      $��* �6'   $	��200��61   $	��350��6@   �� 5*      $��5 �6'   �� 5+      $��* �6'   .+�5.      � $��0��6   
������� 5/        	ArrowDown	 �� Stretch
 J� RenderTransformOrigin  ArrowUp��%  &	       &	 �     �� 50      .R�51      F� $��10��6#   $��10��6.   �����h_�
h��
���
P0�
�(��
xe�
� �P�N	� �0�襣
��	��.
P��	�(,
�X�	Ѹи	��	� �Eh3� �E��R	�x��	h��
�@�-
xo:�h_�
h��
$	 Uniform=�6"  
���   �$
 0.5,0.5&�6A  52      �� 53      .R�54      F� $��10��6#   $��10��6.   �������,��8��@�,�/:��^�,��<��(��+�ǃ �(��+^��Ho-��R �0&�- ����8�- ��ѐ@.�AN �(t/-��(t/�� ��Q-.��8��z�-d�����,��8�$	 Uniform=�6,  
���   �$
 0.5,0.5&�6K  55      56      .+�58      �� .+�59      �� $��0,8,0,0q�6    .��61   Tool67   �� 5:       �� Orientation$ 
Horizontal=�6%   $��5,0,0,0q�6>   .+�5;      	� -   5         	�Name$ RadioButton_TilePlacer��5;   *   $��0,5,15,5q�6J   .��6{   Tile placer6�   	� 5<      -   5        $ RadioButton_TileSelector��5<   *   $��0,5,15,5q�6L   .��6}   Tile selector6�   	� 5=      -   5        $ RadioButton_Entities��5=   *   $��0,5,15,5q�6H   .��6y   
Entities6�   5>      �� 5@      $��0,8,0,0q�6    .��61   
Tile layer6=   �� 5A      -   5         ��Name$ TileLayerComboBox��5A   #   $��	12,6,12,6q�6>   $��16(�6�   � 5C      $��
12,0,12,12q�6    5D      �� 5E   "   $��2* �63   6@   �� 5F   "   $��2* �63   6@   �� 5G   "   $��2* �63   6@   �� 5H   "   $��1* �63   6@   �� 5I   "   $��1* �63   6@   .+�5K      �� -   5        $��0��5K   <   $��0,0,0,0q�6L   $��0,0,0,3q�6]   .��6o   	Add new6x   �� 5L      -   5        $��1��5L   =   $��4,0,0,0q�6M   $��0,0,0,3q�6^   .��6p   Edit6v   �� 5M      -   5        $��2��5M   ?   $��4,0,0,0q�6O   $��0,0,0,3q�6`   .��6r   Remove6z   �� 5N      -   5        $��3��5N   ?   $��4,0,0,0q�6O   .��5O   "   �� $��10q�61   #��[ 6=   5P      �� 5Q      -   5        $��4��5Q   A   $��4,0,0,0q�6Q   .��5R   "   �� $��10q�61   #��[ 6=   5S      5T      �� 5V      $��0,8,0,0q�6    .��61   	Tileset6:   � 5W      -   5        $ GridContainer��5W      .+�5X      �� 
�������$
��Auto=�6=   $
��Auto=�6b   $��Stretch=�6�   $	��260��6�   $��
12,6,12,12q�6�   .��5Y   "   � -   5        $ TilesetGrid��5Y   '   
�������.+�5Z   &   �� -   5        $��TilesetSelector��5Z   0    K� ZIndex$ 10��6G   $��16��6Y   $��16��6d   
�������$
��Left=�6   $	��Top=�6�   $y�Hidden=�6�   6�   �� 5[   &   -   5        $��TilesetSelectedArea��5[   0   $ 9��6K   $��32��6\   $��32��6g   
�������$
��Left=�6�   $	��Top=�6�   $y�Hidden=�6�   6�   5\   #   5]      5^      5_      5`      �� 5a      $��1��6   $��5��6/   $��Stretch=�69   � 5c      $��2��6   .+�5d      �� -    5         ��Name$ EditorScroller��5d   M      XEditor.AdvancedZooming   KeepInCenter . $��Visible=�6e   $��Visible=�6�   .��5e      � -!   5        $ EditorMargin��5e       � LayoutTransform 5f      �� 5g   "   -"   5        EditorScale�� �� CenterX$ 0Q�5g   F    �� CenterY$ 0Q�6R    �� ScaleX$ 1Q�6^    �� ScaleY$ 1Q�6i   .+�5i      �� -#   5        $��
EditorGrid��5i   &   
������� �� EdgeMode$ Aliased=�6�   .+�5j   "   �� -$   5        $��Selector��5j   ,   $ 10��6<   $��17��6N   $��17��6Y   
���   �$
��Left=�6t   $	��Top=�6�   6�   5k      5l      5n      5o      5p      5q      �� 5r   
   $��24��6   $��Bottom=�6    $��4,0,4,0q�6;   .@�5s      �� -%   5         ��Name$ StatusBarTextLeft��5s      $��Loaded��67   6J   �� 5t      -&   5        $ StatusBarTextRight��5t      $�� ��68   $��Right=�6C   6a   5u      5v      5w      