   M S B A M L   `   `   `  ����J  FDocumentManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:DocumentManagerDocumentManager  5   	   "    DocumentManager.ManageCities   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:DocumentManager     5   	     � Title$  ManageCities��5   	   $	��450��6   $	��800��6+    � 	Resources 5	       �� MergedDictionaries �� Source�� 5
   
    5      �� 5      $6 0/DocumentManager;component/css/ButtonsStyle.xaml?�6%   �� 5      $4 ./DocumentManager;component/css/LabelStyle.xaml?�6%   %    5      .��5      � ��5   
   �� 5       �� EndPoint$ 0.5,1&�6"    � MappingMode$ RelativeToBoundingBox=�63    �� 
StartPoint$ 0.5,0&�6W   .��5      �  � Color$ 	#FFFDFCFB��6   � 5      $ 	#FFE2D1C3��6    � Offset$ 1Q�61   5      	 � RowDefinitions	 5   
   �� 5      $	��65* �6   5      �� 5      $	��35* �6   .+�5   
   &   System.Windows.Controls.DataGrid  -   5        
  Name$
 
gridCities��5   ,   $��0��6   $��10q�6       
IsReadOnly . 6�   �� 5   
   -   5         ��Name$ txtCity��5      $
��Left=�6#   $��23��6>   $��21,47.2,0,0q�6J   $��1��6_    �� TextWrapping$
 Wrap=�6l   $	��Top=�6�   $	��120��6�    �� 
IsReadOnly . 6�   �� 5   
   -   5        $ txtState��5      $
��Left=�6$   $��23��6?   $��165,47.2,0,0q�6K   $��1��6a   $
 Wrap=�6n   $	��Top=�6�   $	��120��6�   �� 5    
   -   5        $ 
txtPincode��5       $
��Left=�6&   $��23��6A   $��458,48.2,0,0q�6M   $��1��6c   $
 Wrap=�6p   $	��Top=�6�   $	��120��6�   �� 5!   
   -   5        $ 
txtStdCode��5!      $
��Left=�6&   $��23��6A   $��605,48.2,0,0q�6M   $��1��6c   $
 Wrap=�6p   $	��Top=�6�   $	��120��6�   �� 5"   
   $
��City��6   $
��Left=�6   $��
21,9.2,0,0q�6:   $��1��6N   $	��Top=�6[   �� 5#   
   $��Pincode��6   $
��Left=�6"   $��458,10.2,0,0q�6=   $��1��6S   $	��Top=�6`   $��27��6x   �� 5$   
   $��State��6   $
��Left=�6    $��165,9.2,0,0q�6;   $��1��6P   $	��Top=�6]   $��29��6u   �� 5%   
   $��Std Code��6   $
��Left=�6#   $��607,10.2,0,0q�6>   $��1��6T   $	��Top=�6a   $��29��6y   �� 5&   
   -   5         ��Name$ 	lblStatus��5&      $��Center=�6#      StatusLabelStyle#��[  6G   $
��Left=�6q   $��21,116.2,0,0q�6�   $��1��6�   $	��Top=�6�   $	��749��6�   �� 5'   
   -   5        $
��Save��5'      $
��Left=�6    $��271,80.2,0,0q�6;   $��1��6Q   $	��Top=�6^   $��80��6v     UpdateButtonStyle#��[ 6�   6�   �� 5(   
   -   5        $��Delete��5(      $
��Left=�6"   $��391,80.2,0,0q�6=   $��1��6S   $	��Top=�6`   $��80��6x     DeleteButtonStyle#��[ 6�   6�   �� 5)   
   -	   5        $ 
txtCountry��5)      $
��Left=�6&   $��23��6A   $��311,47.2,0,0q�6M   $��1��6c   $
 Wrap=�6p   $	��Top=�6�   $	��120��6�   �� 5*   
   $��Country��6   $
��Left=�6"   $��312,9.2,0,0q�6=   $��1��6R   $	��Top=�6_   $��29��6w   5,      5-      