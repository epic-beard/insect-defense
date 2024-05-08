public static class UiSfx {
    public static readonly string tower_select_list_collapse = "tower_select_list_collapse_sfx_0";
    public static readonly string context_panel_collapse = "context_panel_collapse_sfx_0";
    public static readonly string rocker_switch = "rocker_switch_sfx_0";
    public static readonly string rocker_switch_fail = "rocker_switch_fail_sfx_0";
    public static readonly string dropdown_active = "dropdown_active_sfx_0";
    public static readonly string dropdown_event_changed = "dropdown_event_changed_sfx_0";
    public static readonly string settings_open = "settings_open_sfx_0";
    public static readonly string settings_tab_active = "settings_tab_active_sfx_0";
    public static readonly string speed_dial_click = "speed_dial_click_sfx_0";
    public static readonly string game_start = "game_start_sfx_0";
    public static readonly string died = "died_sfx_0";

    public static void PlaySfx(string name) {
        AudioManager.Instance.Play(name);
    }
}
