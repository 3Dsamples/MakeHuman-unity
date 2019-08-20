//==============================================================================================
/*!バックグラウンド.
	@file  CTitleBG
	
	(counter SJIS string )
*/
//==============================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KS;

public class CTitleBG : CBG {
	protected override void createMesh(float width,float height) {
		m_vertices[0] = new Vector3(-width * 0.5f, height * 0.5f,50.0f);
		m_vertices[1] = new Vector3( width * 0.5f, height * 0.5f,50.0f);
		m_vertices[2] = new Vector3(-width * 0.5f,-height * 0.5f,50.0f);
		m_vertices[3] = new Vector3( width * 0.5f,-height * 0.5f,50.0f);
		
		m_uvs[0] = new Vector2(0.0f,1.0f);
		m_uvs[1] = new Vector2(1.0f,1.0f);
		m_uvs[2] = new Vector2(0.0f,0.0f);
		m_uvs[3] = new Vector2(1.0f,0.0f);

		Color32	color = m_color;
		m_colors[0] = color;
		m_colors[1] = color;
		color.a = 0;
		m_colors[2] = color;
		m_colors[3] = color;
		
		m_triangles[0] = 0;
		m_triangles[1] = 1;
		m_triangles[2] = 2;
		m_triangles[3] = 1;
		m_triangles[4] = 3;
		m_triangles[5] = 2;
		
		m_bounds	= new Bounds(Vector3.zero,new Vector3(65536f,65536f,65536f));

		m_mesh.vertices = m_vertices;
		m_mesh.uv = m_uvs;
		m_mesh.colors32 = m_colors;
		m_mesh.triangles = m_triangles;
		m_mesh.bounds = m_bounds;
		
		gameObject.layer = (int) m_eLayerId;
		transform.position = Vector3.zero;
		transform.localRotation = Quaternion.identity;
	}
	public override Color	color {
		get {
			return m_color;
		}
		set {
			if (m_color == value) {
				return;
			}
			m_color = value;
			createMesh(m_size.x,m_size.y);
		}
	}
};
