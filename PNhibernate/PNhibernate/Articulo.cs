using System;

namespace Serpis.Ad
{
	public partial class Articulo : Gtk.Dialog
	{
		
			public virtual long Id {get; set;}
			public virtual string Nombre {get; set;}
			public virtual decimal Precio {get; set;}
			public virtual Categoria Categoria {get; set;}
	}
}

