   M S B A M L   `   `   `  ����J  FDocumentManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:DocumentManagerDocumentManager  5   	   #    DocumentManager.DataLayerTest   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:DocumentManager     5   	     � Title$  DataLayerTest��5   	   $	��800��6   $	��800��6,    � WindowStartupLocation$ CenterScreen=�68   .��5	      � .+�5
   
   ��  �� Orientation$ Vertical=�6   .+�5      �� -   5         ��Name$ txtCompanyName��5      $
��Left=�6.   $��23��6I    �� TextWrapping$
 Wrap=�6V   $	��Top=�6k   $	��120��6�   �� 5      -   5        $��
AddCompany��5      $
��Left=�6*   $	��Top=�6F   $��90��6^   6i   �� 5      -   5        $
��Edit��5      $
��Left=�6$   $	��Top=�6@   $��75��6X   6c   �� 5      -   5        $ txtCompanyId��5      $y�	Collapsed=�6,   �� 5      -   5        $��Delete��5      $
��Left=�6&   $	��Top=�6B   $��75��6Z   6e   �� 5      -   5        $��Get All��5      $
��Left=�6=   $	��Top=�6X   $��75��6p   �� 5      -   5        $��Get Company By Company Id��5      6e   �� 5      -   5        $��Add Address��5      $
��Left=�6+   $	��Top=�6F   $	��140��6^    J� RenderTransformOrigin$ 1.003,0.787&�6j   6�   �� 5      -	   5        $��Delete Address��5      $
��Left=�6.   $	��Top=�6I   $	��115��6a   6m   �� 5      -
   5        $��Get Address by company Id��5      $
��Left=�69   $	��Top=�6T   $	��185��6l   $��22��6x   6�   �� 5      -   5        $��Get Address by Address Id��5      $
��Left=�69   $	��Top=�6T   $	��187��6l   6x   �� 5      -   5        $��Add City��5      $
��Left=�6(   $	��Top=�6C   $��75��6[   6f   �� 5      -   5        $��Get all Cities��5      $
��Left=�6.   $	��Top=�6I   $��96��6a   6l   �� 5      -   5        $��	Edit City��5      $
��Left=�6)   $	��Top=�6D   $��75��6\   6g   �� 5      -   5        $��Delete City��5      $
��Left=�6+   $	��Top=�6F   $��75��6^   6i   �� 5      -   5        $��Get City By Name��5      $
��Left=�60   $	��Top=�6K   $	��130��6c   6o   �� 5      -   5        $��Add Contact��5      $
��Left=�6+   $	��Top=�6F   $��95��6^   6i   �� 5      -   5        $��Edit contact��5      $
��Left=�6,   $	��Top=�6G   $��90��6_   6j   �� 5      -   5        $��Delete contact��5      $
��Left=�6.   $	��Top=�6J   $��94��6b   6m   �� 5      -   5        $��Get contact by company id��5      $
��Left=�69   $	��Top=�6U   $	��170��6m   6y   �� 5       -   5        $��GetDocTypeIdByName��5       6U   �� 5!      -   5        $��GetSettings��5!      6M   �� 5"      -   5        $��UpdateSettings��5"      6S   �� 5#      -   5        $��GetDocuments��5#      $��
0,0,-0.4,0q�6M   �� 5$      -   5        $��Add Document��5$      6N   �� 5%      -   5        $��Delete Document��5%      6T   �� 5'      -   5        $��Get all  Models��5'      6/   �� 5(      -   5        $��	Add Model��5(      6)   �� 5)      -   5        $��Update Model��5)      6,   �� 5*      -   5        $��Delete Model��5*      6,   �� 5,      -   5        $��Get All Services��5,      60   �� 5-      -    5        $��Add Service��5-      6+   �� 5.      -!   5        $��Update Service��5.      6.   �� 5/      -"   5        $��Delete Service��5/      6.   50      51      52      