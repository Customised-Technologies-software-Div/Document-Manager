   M S B A M L   `   `   `  ����J  FDocumentManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:DocumentManagerDocumentManager  5   	        DocumentManager.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:DocumentManager     5   	     � Title$  
MainWindow��5   	    � WindowStartupLocation$ CenterScreen=�6   $
��1500��6A   $	��700��6N    � 	Resources 5	       �� MergedDictionaries �� Source�� 5
   
    5      �� 5      $6 0/DocumentManager;component/css/ButtonsStyle.xaml?�6%   6a   �� 5      $4 ./DocumentManager;component/css/LabelStyle.xaml?�6%   �� 5      $7 1/DocumentManager;component/css/DataGridStyle.xaml?�6%   �� 5      $3 -/DocumentManager;component/css/GridStyle.xaml?�6%   %    5      .��5      � .+�5   
   �� -   5         ��Name$ selectedCompanyId��5      $y�	Collapsed=�6+   6D   �� 5   
   -   5        $ selectedCompanyName��5      $y�	Collapsed=�6-   6F   �� 5   
   -   5         ��Name$ cmbCompanies��5      $
��Left=�6)   $��	30,21,0,0q�6D   $	��Top=�6W   $	��330��6o   �� 5   
   -   5         ��Name$ btnAddCompany��5      $	��Add��6(   $
��Left=�66   $��	30,55,0,0q�6Q   $	��Top=�6d      AddButtonStyle#��[  6|   5      �� 5   
   -   5         ��Name$ txtCompanyName��5      $
��Left=�6*   $��	30,15,0,0q�6E   $	��Top=�6X   $	��330��6p   $y�	Collapsed=�6|   6�   �� 5   
   -   5        $ btnEditCompany��5      $
��Save��6)   $
��Left=�69   $��
103,55,0,0q�6T   $	��Top=�6h     UpdateButtonStyle#��[ 6�   5      �� 5   
   -   5        $ btnDeleteCompany��5      $��Delete��6+   $
��Left=�6<   $��
180,55,0,0q�6W   $	��Top=�6k     DeleteButtonStyle#��[ 6�   �� 5   
   -   5        $ btnCancelCompany��5      $��Cancel��6+   $
��Left=�6<   $��
255,55,0,0q�6W   $	��Top=�6k   {�.    CancelButtonStyle#��[ 6�   �� 5   
   -	   5        $
��Left=�5      $��
330,55,0,0q�6,   $	��Top=�6@   $��30��6X   $��22��6c     RefreshButtonStyle#��[ 6�   .��5      �� $��/images/refresh-icon.png��6   $��19��66   $��15��6B   6O   5       �� 5!   
   -
   5        $ lblCompanyStatus��5!        StatusLabelStyle#��[ 6*   $
��Left=�6T   $��	30,90,0,0q�6o   $	��Top=�6�   �� 5"   
   $��Address Line 1��6   $
��Left=�6)   $��
30,128,0,0q�6D   $	��Top=�6X   	 J� RenderTransformOrigin$	 -0.282,0.667&�6p   �� 5#   
   -   5        $ txtAddress1��5#      $
��Left=�6'   $��23��6B   $��135,135,0,0q�6N   
 �� TextWrapping$

 Wrap=�6c   $	��Top=�6x   $	��225��6�   .��6�   ��5$      �� 5%       �� EndPoint$ 0.5,1&�6&    � MappingMode$ RelativeToBoundingBox=�67    �� 
StartPoint$ 0.5,0&�6[   .��5&      �  � Color$ 	#80C7C7C7��6#    � Offset$ 1Q�65   � 5'      $ 	#A6D7D7D7��6#   $ 0.008Q�65   � 5(      $ 	#FFFFFEE1��6#   $ 0.883Q�65   � 5)      $ 	#FFFFFEE1��6#   $ 0.125Q�65   � 5*      $ 	#A6D7D7D7��6#   $ 0.971Q�65   5+      .��5,   "   5-      �� 5.   
   $��Address Line 2��6   $
��Left=�6)   $��
30,163,0,0q�6D   $	��Top=�6X   $	 -0.282,0.667&�6p   �� 5/   
   -   5        $ txtAddress2��5/      $
��Left=�6'   $��23��6B   $��135,169,0,0q�6N   $

 Wrap=�6c   $	��Top=�6x   $	��225��6�   �� 50   
   $��Address Line 3��6   $
��Left=�6)   $��
30,203,0,0q�6D   $	��Top=�6X   $	 -0.282,0.667&�6p   �� 51   
   -   5        $ txtAddress3��51      $
��Left=�6'   $��23��6B   $��135,209,0,0q�6N   $

 Wrap=�6c   $	��Top=�6x   $	��225��6�   �� 52   
   $
��City��6   $
��Left=�6   $��
30,246,0,0q�6:   $	��Top=�6N   $	 -0.282,0.667&�6f   �� 53   
   -   5        $ txtCity��53      $
��Left=�6$   $��23��6?   $��135,246,0,0q�6K   $

 Wrap=�6`   $	��Top=�6u   $	��155��6�   6�   �� 54   
   $��State��6   $
��Left=�6    $��
30,280,0,0q�6;   $	��Top=�6O   $	 -0.282,0.667&�6g   �� 55   
   -   5        $��cmbState��55      $
��Left=�6$   $��135,286,0,0q�6?   $	��Top=�6T   $	��155��6l   6x   �� 56   
   $��Country��6   $
��Left=�6"   $��
30,323,0,0q�6=   $	��Top=�6Q   $	 -0.282,0.667&�6i   �� 57   
   -   5        $ 
txtCountry��57      $
��Left=�6&   $��23��6A   $��135,325,0,0q�6M   $

 Wrap=�6b   $	��Top=�6w   $	��225��6�   $	 0.492,0.565&�6�   �� 59   
   $��Pincode��6   $
��Left=�6"   $��
30,359,0,0q�6=   $	��Top=�6Q   $	 -0.282,0.667&�6i   �� 5:   
   -   5        $ 
txtPincode��5:      $
��Left=�6&   $��23��6A   $��135,362,0,0q�6M   $

 Wrap=�6b   $	��Top=�6w   $	��225��6�   $	 0.492,0.565&�6�   �� 5;   
   $��Phone Number��6   $
��Left=�6'   $��
30,405,0,0q�6B   $	��Top=�6V   $	 -0.282,0.667&�6n   �� 5<   
   -   5        $ txtPhone��5<      $
��Left=�6%   $��45��6@   $��135,404,0,0q�6L   $

 Wrap=�6a   $	��Top=�6v   $	��225��6�   �� 5=   
   $��
State Code��6   $
��Left=�6%   $��
30,454,0,0q�6@   $	��Top=�6T   $	 -0.282,0.667&�6l   �� 5>   
   -   5        $ txtStateCode��5>      $
��Left=�6(   $��23��6C   $��135,460,0,0q�6O   $

 Wrap=�6d   $	��Top=�6y   $	��225��6�   $	 0.492,0.565&�6�   $��0��6�   �� 5?   
   $��GST No.��6   $
��Left=�6"   $��
30,495,0,0q�6=   $	��Top=�6Q   $	 -0.282,0.667&�6i   �� 5@   
   -   5        $ txtGSTNo��5@      $
��Left=�6$   $��23��6?   $��135,495,0,0q�6K   $

 Wrap=�6`   $	��Top=�6u   $	��225��6�   $	 0.492,0.565&�6�   �� 5A   
   -   5        $��18(�5A      $
��Left=�6   $��300,246,0,0q�6:   $	��Top=�6O   #��[  6h   .��5B      �� $��/images/Add-icon.png��6   64   5C      �� 5D   
   -   5        $��Manage��5D      $
��Left=�6"   $��300,286,0,0q�6=   $	��Top=�6R   $
��60px��6j   $
��25px��6w     PrimaryButtonStyle#��[ 6�   6�   �� 5E   
   -   5        $ btnPrev��5E      $
��Left=�6#   $��
75,534,0,0q�6>   $	��Top=�6R   $��75��6j   $��25��6u   #��[ 6�   {�.  .��5F      �� $��/images/Left-arrow.png��6   66   5G      �� 5H   
   -   5        $ txtAddressInfo��5H      $
��Left=�6*   $��23��6E   $��165,535,0,0q�6Q   $

 Wrap=�6f   $	��Top=�6{   $��35��6�   $��0 / 0��6�    �� 
IsReadOnly . 6�   �� 5I   
   -   5        $ btnNext��5I      $
��Left=�6"   $��215,534,0,0q�6=   $	��Top=�6R   $��75��6j   $��25��6u   {�.  #��[ 6�   .��5J      �� $��/images/Right-arrow.png��6   67   5K      �� 5L   
   -   5        $ btnAddAddress��5L      $	��Add��6(   $
��Left=�66   $��
30,589,0,0q�6Q   $	��Top=�6e   $��70��6}   #��[  6�   6�   �� 5M   
   -   5        $ btnSaveAddress��5M      $
��Save��6)   $
��Left=�68   $��115,589,0,0q�6S   $��70��6h   $	��Top=�6s   #��[ 6�   6�   �� 5N   
   -   5        $ btnDeleteAddress��5N      $��Delete��6+   $
��Left=�6<   $��200,589,0,0q�6W   $��70��6l   $	��Top=�6w   #��[ 6�   6�   �� 5O   
   -   5        $ btnCancelAddress��5O      $��Cancel��6+   $
��Left=�6<   $��285,589,0,0q�6W   $��70��6l   $	��Top=�6w   #��[ 6�   6�   �� 5S   
   $��	Template ��6   $
��Left=�6$   $��
385,15,0,0q�6?   $	��Top=�6S   $��35��6k   $��75��6w   �� 5T   
   -   5        $ txtTemplateFolderPath��5T       .  �� IsReadOnlyCaretVisible . $
��Left=�6a   $��
475,15,0,0q�6|   $	��Top=�6�   $	��280��6�   $��38��6�   $

 Wrap=�6�   �� 5U   
   -   5        $	��...��5U      $
��Left=�6   $��
768,15,0,0q�6:   $	��Top=�6N     BrowseButtonStyle#��[ 6�   �� 5V   
   -    5        $��Open Template Folder��5V      $
��Left=�61   $��
385,73,0,0q�6L   $	��Top=�6`   $	��190��6x   #��[ 6�   �� 5W   
   -!   5        $��Create New Template��5W      $
��Left=�6/   $��
595,73,0,0q�6J   $	��Top=�6^   $	��199��6v   #��[ 6�   �� 5X   
   -"   5        $ cmbTemplateFolders��5X      $
��Left=�6/   $��389,126,0,0q�6J   $	��Top=�6_   $	��156��6w   $	 
0.55,0.435&�6�   �� 5Y   
   -#   5        $ cmbTemplateFiles��5Y   G   $
��Left=�6a   $��570,126,0,0q�6|   $	��Top=�6�   $	��224��6�   &   System.Windows.Controls.DataGrid  5Z   
   -$   5          Name$ gridContacts��5Z      $
��Left=�6)   $	��254��6D   $��392,173,0,0q�6Q   $	��Top=�6f   $	��405��6~      AutoGenerateColumns .    
IsReadOnly . 6,  �� 5\   
   -%   5        $ lblCompanyId��5\      $y�	Collapsed=�6&   �� 5^   
   $
��Name��6   $
��Left=�6   $��392,449,0,0q�6:   $	��Top=�6O   �� 5_   
   $��Email��6   $
��Left=�6    $��570,447,0,0q�6;   $	��Top=�6P   �� 5`   
   $��Phone��6   $
��Left=�6    $��695,449,0,0q�6;   $	��Top=�6P   $	 
4.38,0.931&�6h   �� 5a   
   -&   5        $ txtContactName��5a   �   $
��Left=�6   $��37��6-   $��392,492,0,0q�69   $

 Wrap=�6N   $	��Top=�6c   $	��103��6{   6�   �� 5b   
   -'   5        $ txtContactEmail��5b   �   $
��Left=�6   $��37��6-   $��515,492,0,0q�69   $

 Wrap=�6N   $	��Top=�6b   $	��160��6z   $	 0.499,0.486&�6�   6�   �� 5c   
   -(   5        $ txtContactPhone��5c   �   $
��Left=�6   $��37��6-   $��695,492,0,0q�69   $

 Wrap=�6N   $	��Top=�6b   $	��102��6z   $	 0.485,0.486&�6�   6�   �� 5d   
   -)   5        $	��Add��5d      $
��Left=�6   $��460,540,0,0q�6:   $	��Top=�6O   $��80��6g   #��[  6r   6�   �� 5e   
   -*   5        $
��Save��5e      $
��Left=�6    $��550,540,0,0q�6;   $	��Top=�6P   $��80��6h   #��[ 6s   6�   �� 5f   
   -+   5        $��Delete��5f      $
��Left=�6"   $��640,540,0,0q�6=   $	��Top=�6R   $��80��6j   #��[ 6u   6�   �� 5g   
   -,   5        $ lblContactStatus��5g      #��[ 6*   $
��Left=�6T   $��392,578,0,0q�6o   $	��Top=�6�   $	��405��6�     5h   
   --   5        $ gridDocuments��5h      $	��279��6*   $��842,173,45.6,0q�67   $	��Top=�6O    .  . 6�   �� 5i   
   -.   5        $��Delete Document��5i      $
��Left=�6+   $��1180,467,0,0q�6F   $	��Top=�6\   $	��145��6t   #��[ 6�   �� 5j   
   -/   5        $ txtDocumentSerialNumber��5j      $
��Left=�63   $��23��6N   $��973,506,0,0q�6Z   $

 Wrap=�6o   $	��Top=�6�   $	��120��6�   6�   �� 5k   
   $��S.No.��6   $
��Left=�6    $��904,504,0,0q�6;   $	��Top=�6P   $	 0.579,-0.181&�6h   �� 5l   
   -0   5        $ txtRef��5l      $
��Left=�6"   $��23��6=   $��1245,506,0,0q�6I   $

 Wrap=�6_   $��B��6s   $	��Top=�6|   $	��120��6�   �� 5m   
   $	��Ref��6   $
��Left=�6   $��1184,504,0,0q�69   $	��Top=�6O   $��32��6g   �� 5n   
   -1   5        $
��Left=�5n      $��1385,506,0,0q�6,   $	��Top=�6B   $��30��6Z   $��23��6e   #��[ 6�   .��5o      �� $��/images/save-image.png��6   $��20��64   $��25��6@   6N   5p      �� 5q   
   -2   5        $ txtTemplateFilePath��5q      $
��Left=�6/   $��39��6J   $��1016,120,0,0q�6V   $

 Wrap=�6l   $	��Top=�6�   $	��432��6�   �� 5r   
   $��Template File Path��6   $
��Left=�6-   $��842,126,0,0q�6H   $	��Top=�6]   �� 5s   
   -3   5        $ txtSaveFilePath��5s      $
��Left=�6+   $��39��6F   $��1016,65,0,0q�6R   $

 Wrap=�6g   $	��Top=�6|   $	��432��6�   �� 5t   
   $��Document File Path��6   $
��Left=�6-   $��
842,66,0,0q�6H   $	��Top=�6\   �� 5u   
   $��Document Root Folder��6   $
��Left=�6/   $��
841,15,0,0q�6J   $	��Top=�6^   $��14(�6v   $��39��6�   $	��156��6�   �� 5v   
   -4   5        $ txtDocumentFolderPath��5v      $
��Left=�61   $��35��6L   $��1015,15,0,0q�6X   $

 Wrap=�6m   $	��Top=�6�   $	��310��6�   �� 5w   
   -5   5        $	��...��5w      $
��Left=�6   $��1340,15,0,0q�6:   $	��Top=�6O   #��[ 6g   6�   �� 5x   
   -6   5        $
��Open��5x      $
��Left=�6    $��1380,15,0,0q�6;   $	��Top=�6P   $��67��6h   $	 0.373,0.787&�6�   $��35��6�   #��[ 6�   �� 5y   
   $
��Date��6   $
��Left=�6   $��842,463,0,0q�6:   $	��Top=�6O   $	 -0.282,0.667&�6g   $��50��6�   (  "System.Windows.Controls.DatePicker  5{   
   -7   5        $��txtDate��5{   Q   $��25��6   $
��Left=�6!   $��915,467,0,0q�6<   $	��Top=�6`   $	��175��6x   �� 5|   
   -8   5        $
��Left=�5|      $��1132,467,0,0q�6,   $	��Top=�6B   $��25��6[   $��28��6g   #��[ 6�   .��5}      �� $��/images/Refresh-icon.png��6   5~      �� 5�   
   -9   5        $��Generate��5�      $��30(�6$   $
��Left=�62   $��852,582,0,0q�6M   $	��Top=�6b   $	��218��6z   $��50��6�   ���	#FF8FE34D6�   �� 5�   
   -:   5         ��Name$ btnOpenToggle��5�      $
��Open��6*   $
��Left=�69   $��1087,596,0,0q�6T   $	��Top=�6j    d� 	IsChecked$
 TrueY�6�   �� 5�   
   -;   5        $ lblAddressStatus��5�   :   #��[ 6   $
��Left=�6T   $��
30,632,0,0q�6o   $	��Top=�6�   $	��325��6�   �� 5�   
   -<   5        $��GST Service Invoice��5�      $��15(�6/   $
��Left=�6=   $��1184,582,0,0q�6X   $	��Top=�6n   $	��145��6�   $��32��6�   #��[ 6�   6�   �� 5�   
   -=   5        $��Open Document��5�      #��[ 6)   $
��Left=�6U   $��1340,467,0,0q�6p   $	��Top=�6�   $	��107��6�   6�   �� 5�   
   $��Service Invoice No��6   $
��Left=�6-   $��842,539,0,0q�6H   $	��Top=�6]   �� 5�   
   ->   5        $ txtServiceInvoiceNo��5�      $
��Left=�6/   $��23��6J   $��970,540,0,0q�6V   $

 Wrap=�6k   $	��Top=�6   $	��120��6�   �� 5�   
   $��Tax Invoice No��6   $
��Left=�6)   $��1132,540,0,0q�6D   $	��Top=�6Z   �� 5�   
   -?   5        $ txtTaxInvoiceNo��5�      $
��Left=�6+   $��23��6F   $��1245,540,0,0q�6R   $

 Wrap=�6h   $	��Top=�6|   $	��120��6�   5�      5�      