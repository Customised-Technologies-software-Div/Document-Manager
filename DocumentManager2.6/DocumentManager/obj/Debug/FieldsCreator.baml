   M S B A M L   `   `   `  ����I  Eunvell.ReoGrid, Version=2.2.0.0, Culture=neutral, PublicKeyToken=nullG4clr-namespace:unvell.ReoGrid;assembly=unvell.ReoGridunvell.ReoGrid  5   
   J FDocumentManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:DocumentManagerDocumentManager 5   	   #   DocumentManager.FieldsCreator   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Crg4clr-namespace:unvell.ReoGrid;assembly=unvell.ReoGrid     5   
   /localclr-namespace:DocumentManager     5   	     � Title$  FieldsCreator��5	   	   $
��1920��6   $
��1080��6,    � WindowState$ 	Maximized=�6:    � 	Resources 5
       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $6 0/DocumentManager;component/css/ButtonsStyle.xaml?�6%   6a   %    5      .��5      � ��5   
   �� 5       �� EndPoint$ 0.5,1&�6"    � MappingMode$ RelativeToBoundingBox=�63    �� 
StartPoint$ 0.5,0&�6W   .��5      �  � Color$ 	#FFFDFCFB��6   � 5      $ 	#FFE2D1C3��6   	 � Offset$	 1Q�61   5      
 � RowDefinitions
 5   
   �� 5      $��8* �6   6*   �� 5      $	��57* �6   6+   �� 5      $	��35* �6   6+    � ColumnDefinitions 5   
   �� 5      $	��30* �6   6-   �� 5      $	��70* �6   6-   .+�5!   
   � $��0��6   $��0��6   $��2��6,   .+�5"      ��  �� Orientation$ 
Horizontal=�6   .+�5#      �� $��Template Name��6   $��10q�60   $	��130��6<   �� 5$      -   5         ��Name$ txtTemplateName��5$   (   $
��Auto��6   $��10q�6A   $	��300��6M   �� 5%      $��Select Template File��6   $��10q�67   $	��140��6C   �� 5&      -   5        $ txtSelectTemplate��5&   (   $
��Auto��6   $��10q�6C   $	��600��6O   �� 5'      -   5           BrowseButtonStyle#��[  5'      $
��Auto��6D   $	��...��6R   $��10q�6`   $��30��6l   6w   5(      5)      #   unvell.ReoGrid.ReoGridControl  5*   
   -   5          Name$ reoGridControl��5*      $��1��64   $��0��6A   $��2��6Q   6e   � 5+   
   $��2��6   $��0��6    5,      �� 5-      $	��30* �6#   61   �� 5.      $	��80* �6#   61   
 50      �� 51      $	��20* �6    6/   �� 52      $	��20* �6    6/   �� 53      $	��20* �6    6/   �� 54      $	��20* �6    6/   �� 55      $	��20* �6    6/   .+�57      �� $
��Name��6   $��0��6#   $��0��60   $��10q�6@   6N   �� 58      -   5         ��Name$ cmbFieldNames��58      $��10q�6.   $��0��6:   $��1��6G   6Y   �� 5:      $	��Row��6   $��1��6"   $��0��6/   $��10q�6?   6M   �� 5;      -   5        $ txtRow��5;   \   $
��Auto��6   $
��auto��6#   $��16(�61   $��1��6?   $��1��6L   $��10q�6l   6z   �� 5<      $��Column��6   $��2��6%   $��0��62   $��10q�6B   6P   �� 5=      -   5        $ 	txtColumn��5=   ]   $
��Auto��6   $
��auto��6$   $��16(�62   $��2��6@   $��1��6M   $��10q�6p   6~   �� 5>      $��Sheet��6   $��3��6$   $��0��61   $��10q�6A   6O   �� 5?      -   5        $ txtSheet��5?   ]   $
��Auto��6   $
��auto��6$   $��16(�62   $��3��6@   $��1��6M   $��10q�6o   6}   �� 5@      $ 
Horizontal=�6   $��4��62   $��0��6?   $��2��6O   $��Right=�6c   .+�5A      �� -	   5         ��Name$ lblFieldError��5A      
������ 
���  ��$	��200��6a   $
��Wrap=�6m   6�   �� 5B      -
   5          AddButtonStyle#��[ 5B      $��	Add Field��6A   $
��Auto��6U   $	��100��6c   $��10q�6o   $��5q�6{   6�   �� 5C      -   5          PrimaryButtonStyle#��[ 5C      $��Reset��6E   $	��100��6U   $
��Auto��6a   $��10q�6o   $��5q�6{   6�   5D      5F      � 5G   
   $��1��6   $��2��6    
 5H      �� 5I      $	��80* �6    6/   �� 5J      $	��20* �6    6/   .+�5L      &   System.Windows.Controls.DataGrid  -   5          Name$ fieldDataGrid��5L         
IsReadOnly . $��10q�6@   $��0��6L   $��0��6Y   6�   �� 5M      $��1��6   $��0��6&   $ 
Horizontal=�66   .+�5N      �� -   5          UpdateButtonStyle#��[ 5N      $
��Save��6D   $��10q�6S   $	��100��6_   $
��Auto��6k   6y   �� 5O      -   5          CancelButtonStyle#��[ 5O      $��Cancel��6D   $��1��6U   $��10q�6e   $	��100��6r   $
��Auto��6~   6�   5P      5Q      5R      5S      