   M S B A M L   `   `   `  ����B  >XEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:XEditorXEditor  5   	       XEditor.AddEditLayer   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:XEditor     5   	     � Title$  Adding Layer��5   	   $��107.644��6   $��481.211��6/   .��5	      � $��
4, 4, 4, 4q�6    � ColumnDefinitions 5
   
   �� 5      $��* �6   6+   �� 5      $��* �6   6+   �� 5      $��* �6   6+    � RowDefinitions 5   
   �� 5      6   �� 5      6   .+�5   
   �� $��0��6   $��0��6    $��4,0,4,0q�6-   .��6>   Name6D   �� 5   
   -   5         ��Name$ PositionLabel��5      $��1��6'   $��0��67   $��4,0,4,0q�6D   .��6U   Insert position6f   �� 5   
   $��2��6   $��0��6    $��4,0,4,0q�6-   6@   �� 5   
   -   5         ��Name$ 	LayerName��5      $��0��6%   $��1��65   $��4,4,4,4q�6B   6U   �� 5   
   -   5         ��Name$ LayerIndexes��5      $��1��6)   $��1��69   $��4,4,4,4q�6F   6Y   �� 5   
   -   5         ��Name$ ApplyButton��5      $��Create layer��6&   $��2��6=   $��1��6M   $��4,4,4,4q�6[    �� 	IsDefault . 6�   5      5      