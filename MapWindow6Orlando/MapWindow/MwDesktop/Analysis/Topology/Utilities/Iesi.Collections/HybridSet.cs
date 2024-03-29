//********************************************************************************************************
// Product Name: MapWindow.dll Alpha
// Description:  The basic module for MapWindow version 6.0
//********************************************************************************************************
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); 
// you may not use this file except in compliance with the License. You may obtain a copy of the License at 
// http://www.mozilla.org/MPL/  Alternately, you can access an earlier version of this content from
// the Net Topology Suite, which is protected by the GNU Lesser Public License
// http://www.gnu.org/licenses/lgpl.html and the sourcecode for the Net Topology Suite
// can be obtained here: http://sourceforge.net/projects/nts.
//
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF 
// ANY KIND, either expressed or implied. See the License for the specificlanguage governing rights and 
// limitations under the License. 
//
// The Original Code is from the Net Topology Suite
//
// The Initial Developer to integrate this code into MapWindow 6.0 is Ted Dunsford.
// 
// Contributor(s): (Open source contributors should list themselves and their modifications here). 
//
//********************************************************************************************************
/* Copyright � 2002-2004 by Aidant Systems, Inc., and by Jason Smith. */ 
using System;
using System.Collections;
using System.Collections.Specialized;

namespace Iesi.Collections
{
	/// <summary>
	/// Implements a <c>Set</c> that automatically changes from a list to a hash table
	/// when the size reaches a certain threshold.  This is good if you are unsure about
	/// whether you data-set will be tiny or huge.  Because this uses a dual implementation,
	/// iteration order is not guaranteed!
	/// </summary>
	[Serializable]
	public class HybridSet : DictionarySet
	{
		/// <summary>
		/// Creates a new set instance based on either a list or a hash table, depending on which 
		/// will be more efficient based on the data-set size.
		/// </summary>
		public HybridSet()
		{
			InternalDictionary = new HybridDictionary();
		}


		/// <summary>
		/// Creates a new set instance based on either a list or a hash table, depending on which 
		/// will be more efficient based on the data-set size, and
		/// initializes it based on a collection of elements.
		/// </summary>
		/// <param name="initialValues">A collection of elements that defines the initial set contents.</param>
		public HybridSet(ICollection initialValues) : this()
		{
			this.AddAll(initialValues);
		}
	}
}
