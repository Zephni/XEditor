   M S B A M L   `   `   `  ���� B  >XEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:XEditorXEditor      XEditor.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     'localclr-namespace:XEditor     -     � Title$  XEditor��$
��1080��$	��600�� J� 	AllowDrop .  � 	Resources )   #XEditor.EnumMatchToBooleanConverter   EnumConverter��%   &	          .��� 
�������.+�]� $��0,0,0,24q�.+�o� -    o�Name$ DockMenu��$	��Top=�.@�m� $��_File��.@�m� -   $
��_New�� m� InputGestureText$ Ctrl + N��m� -   $��_Open��$ Ctrl + O��m� -    m�Name$ 
Menu_Close��$��_Close��m� -   $ 	Menu_Save��$��_Save��$ Ctrl + S��m� -   $ Menu_SaveAs��$��_Save as��m� -   $ Menu_RecentFiles��$��Recent files��m� -	   $
 Exit��$��_Exit��$ Alt + F4��m� $
��Edit��.@�m� -
   $ Menu_LevelSettings��$��Level Settings��{�.  m� $��Preferences��.@�m� -   $ Menu_SaveAsCompressed��$��Save as compressed�� m� IsCheckable . m� -   $ Menu_ShowGrid��$��	Show grid�� . m� -   $ Menu_DefaultLayers��$��Update default layers��m� -   $ Menu_ViewLayerMode��$��ViewLayerMode��.@�m� -   $ Menu_ViewLayerMode_Solid�� . $��Solid��m� -   $$ Menu_ViewLayerMode_CurrentOnly�� . $��Current only��m� -   $# Menu_ViewLayerMode_FadeOthers�� . $��Fade others��� -    �Name$ EditorContainer��$y�Visible=� � ColumnDefinitions �� $��* �$	��200��$	��350���� $��5 ��� $��* �.+�� $��0��
�������   	ArrowDown	 �� Stretch
 J� RenderTransformOrigin  ArrowUp��%�  &	       &	 �     �� .R�F� $��10��$��10�������h_�
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
$	 Uniform=�
���   �$
 0.5,0.5&��� .R�F� $��10��$��10���������,��8��@�,�/:��^�,��<��(��+�ǃ �(��+^��Ho-��R �0&�- ����8�- ��ѐ@.�AN �(t/-��(t/�� ��Q-.��8��z�-d�����,��8�$	 Uniform=�
���   �$
 0.5,0.5&�.+��� .+��� $��0,8,0,0q�.��Tool��  �� Orientation$ 
Horizontal=�$��5,0,0,0q�.+�	� -    	�Name$ RadioButton_TilePlacer��$��0,5,15,5q�.��Tile placer	� -   $ RadioButton_TileSelector��$��0,5,15,5q�.��Tile selector	� -   $ RadioButton_Entities��$��0,5,15,5q�.��
Entities�� $��0,8,0,0q�.��
Tile layer�� -    ��Name$ TileLayerComboBox��$��	12,6,12,6q�$��16(�� $��
12,0,12,12q� �� $��2* ��� $��2* ��� $��2* ��� $��1* ��� $��1* �.+��� -   $��0��$��0,0,0,0q�$��0,0,0,3q�.��	Add new�� -   $��1��$��4,0,0,0q�$��0,0,0,3q�.��Edit�� -   $��2��$��4,0,0,0q�$��0,0,0,3q�.��Remove�� -   $��3��$��4,0,0,0q�.���� $��10q�#��[ �� -   $��4��$��4,0,0,0q�.���� $��10q�#��[ �� $��0,8,0,0q�.��	Tileset� -   $ GridContainer��.+��� 
�������$
��Auto=�$
��Auto=�$��Stretch=�$	��260��$��
12,6,12,12q�.��� -   $ TilesetGrid��
�������.+��� -   $��TilesetSelector�� K� ZIndex$ 10��$��16��$��16��
�������$
��Left=�$	��Top=�$y�Hidden=��� -   $��TilesetSelectedArea��$ 9��$��32��$��32��
�������$
��Left=�$	��Top=�$y�Hidden=��� $��1��$��5��$��Stretch=�� $��2��.+��� -     ��Name$ EditorScroller��   XEditor.AdvancedZooming   KeepInCenter . $��Visible=�$��Visible=�.��� -!   $ EditorMargin�� � LayoutTransform �� -"   EditorScale�� �� CenterX$ 0Q� �� CenterY$ 0Q� �� ScaleX$ 1Q� �� ScaleY$ 1Q�.+��� -#   $��
EditorGrid��
������� �� EdgeMode$ Aliased=�.+��� -$   $��Selector��$ 10��$��17��$��17��
���   �$
��Left=�$	��Top=��� $��24��$��Bottom=�$��4,0,4,0q�.@��� -%    ��Name$ StatusBarTextLeft��$��Loaded���� -&   $ StatusBarTextRight��$�� ��$��Right=�