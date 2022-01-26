import bpy  
def simplify_fcurves():
    C=bpy.context
    old_area_type = C.area.type
    C.area.type='GRAPH_EDITOR'
    bpy.ops.graph.decimate(mode='ERROR', remove_ratio=1, remove_error_margin=0.01)
    C.area.type=old_area_type    
simplify_fcurves()