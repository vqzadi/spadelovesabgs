using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RobloxFlagInjector
{
    public class MainForm : Form
    {
        private ComboBox _installCombo = new();
        private Button _refreshButton = new();
        private CheckBox _applyToAllCheck = new();
        private DataGridView _grid = new();
        private Button _addRowButton = new();
        private Button _removeRowButton = new();
        private Button _presetButton = new();
        private Button _importButton = new();
        private Button _saveButton = new();
        private Button _loadButton = new();
        private Label _statusLabel = new();

        private List<RobloxInstall> _installs = new();

        public MainForm()
        {
            BuildUi();
            RefreshInstalls();
        }

        private void BuildUi()
        {
            Text = "Roblox FastFlag Injector";
            Width = 780;
            Height = 560;
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new System.Drawing.Size(650, 450);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 90, Padding = new Padding(10) };

            var installLabel = new Label { Text = "Roblox install:", Left = 10, Top = 10, Width = 90 };
            _installCombo.Left = 105;
            _installCombo.Top = 7;
            _installCombo.Width = 420;
            _installCombo.DropDownStyle = ComboBoxStyle.DropDownList;

            _refreshButton.Text = "Refresh";
            _refreshButton.Left = 535;
            _refreshButton.Top = 6;
            _refreshButton.Width = 90;
            _refreshButton.Click += (s, e) => RefreshInstalls();

            _applyToAllCheck.Text = "Apply to ALL detected installs";
            _applyToAllCheck.Left = 105;
            _applyToAllCheck.Top = 38;
            _applyToAllCheck.Width = 260;
            _applyToAllCheck.CheckedChanged += (s, e) => _installCombo.Enabled = !_applyToAllCheck.Checked;

            var infoLabel = new Label
            {
                Text = "Note: Since Sep 2025, Roblox only applies flags on its official Allowlist — anything else is silently ignored (not punished). Presets below are marked [Allowlisted].",
                Left = 10,
                Top = 65,
                Width = 740,
                Height = 20,
                ForeColor = System.Drawing.Color.DimGray,
                Font = new System.Drawing.Font(Font.FontFamily, 7.5f, System.Drawing.FontStyle.Italic)
            };

            topPanel.Controls.Add(installLabel);
            topPanel.Controls.Add(_installCombo);
            topPanel.Controls.Add(_refreshButton);
            topPanel.Controls.Add(_applyToAllCheck);
            topPanel.Controls.Add(infoLabel);

            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 90, Padding = new Padding(10) };

            _addRowButton.Text = "Add Flag";
            _addRowButton.Left = 10;
            _addRowButton.Top = 10;
            _addRowButton.Width = 100;
            _addRowButton.Click += (s, e) => AddBlankRow();

            _removeRowButton.Text = "Remove Selected";
            _removeRowButton.Left = 120;
            _removeRowButton.Top = 10;
            _removeRowButton.Width = 130;
            _removeRowButton.Click += (s, e) => RemoveSelectedRows();

            _presetButton.Text = "Add From Preset...";
            _presetButton.Left = 260;
            _presetButton.Top = 10;
            _presetButton.Width = 140;
            _presetButton.Click += (s, e) => ShowPresetPicker();

            _importButton.Text = "Import From JSON...";
            _importButton.Left = 410;
            _importButton.Top = 10;
            _importButton.Width = 150;
            _importButton.Click += (s, e) => ImportFromJson();

            _loadButton.Text = "Load Current File";
            _loadButton.Left = 10;
            _loadButton.Top = 45;
            _loadButton.Width = 130;
            _loadButton.Click += (s, e) => LoadCurrentFile();

            _saveButton.Text = "Save && Apply";
            _saveButton.Left = 150;
            _saveButton.Top = 45;
            _saveButton.Width = 130;
            _saveButton.BackColor = System.Drawing.Color.LightGreen;
            _saveButton.Click += (s, e) => SaveAndApply();

            _statusLabel.Left = 10;
            _statusLabel.Top = 72;
            _statusLabel.Width = 740;
            _statusLabel.Text = "Ready.";
            _statusLabel.ForeColor = System.Drawing.Color.DarkSlateGray;

            bottomPanel.Controls.Add(_addRowButton);
            bottomPanel.Controls.Add(_removeRowButton);
            bottomPanel.Controls.Add(_presetButton);
            bottomPanel.Controls.Add(_importButton);
            bottomPanel.Controls.Add(_loadButton);
            bottomPanel.Controls.Add(_saveButton);
            bottomPanel.Controls.Add(_statusLabel);

            SetupGrid();

            Controls.Add(_grid);
            Controls.Add(bottomPanel);
            Controls.Add(topPanel);
        }

        private void SetupGrid()
        {
            _grid.Dock = DockStyle.Fill;
            _grid.AllowUserToAddRows = false;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.Columns.Add("Name", "Flag Name");

            var typeCol = new DataGridViewComboBoxColumn
            {
                Name = "Type",
                HeaderText = "Type",
                DataSource = Enum.GetValues(typeof(FlagValueType)),
                FillWeight = 60
            };
            _grid.Columns.Add(typeCol);
            _grid.Columns.Add("Value", "Value");

            _grid.Columns["Name"].FillWeight = 200;
            _grid.Columns["Value"].FillWeight = 150;
        }

        private void RefreshInstalls()
        {
            _installs = RobloxLocator.FindInstalls();
            _installCombo.Items.Clear();

            if (_installs.Count == 0)
            {
                _installCombo.Items.Add("No Roblox installs found");
                _installCombo.SelectedIndex = 0;
                _installCombo.Enabled = false;
                SetStatus("No Roblox installation detected under %LOCALAPPDATA%\\Roblox\\Versions. " +
                          "Make sure Roblox is installed, or check the path manually.", isError: true);
                return;
            }

            _installCombo.Enabled = true;
            foreach (var install in _installs)
                _installCombo.Items.Add(install.ToString());

            _installCombo.SelectedIndex = 0;
            SetStatus($"Found {_installs.Count} Roblox install(s).");
        }

        private RobloxInstall? GetSelectedInstall()
        {
            if (_installCombo.SelectedIndex < 0 || _installs.Count == 0)
                return null;
            return _installs[_installCombo.SelectedIndex];
        }

        private void AddBlankRow()
        {
            _grid.Rows.Add("", FlagValueType.Boolean, "True");
        }

        private void RemoveSelectedRows()
        {
            foreach (DataGridViewRow row in _grid.SelectedRows.Cast<DataGridViewRow>().ToList())
            {
                if (!row.IsNewRow)
                    _grid.Rows.Remove(row);
            }
        }

        private void ShowPresetPicker()
        {
            using var picker = new Form
            {
                Text = "Select a preset flag",
                Width = 500,
                Height = 400,
                StartPosition = FormStartPosition.CenterParent
            };

            var list = new ListBox { Dock = DockStyle.Fill };
            foreach (var preset in FlagPresets.Common)
                list.Items.Add($"{preset.Name} — {preset.Description}");

            var addBtn = new Button { Text = "Add", Dock = DockStyle.Bottom, Height = 35 };
            addBtn.Click += (s, e) =>
            {
                if (list.SelectedIndex >= 0)
                {
                    var preset = FlagPresets.Common[list.SelectedIndex];
                    _grid.Rows.Add(preset.Name, preset.ValueType, preset.SuggestedValue);
                }
                picker.Close();
            };

            picker.Controls.Add(list);
            picker.Controls.Add(addBtn);
            picker.ShowDialog(this);
        }

        private void LoadCurrentFile()
        {
            var install = GetSelectedInstall();
            if (install == null)
            {
                SetStatus("Select a Roblox install first.", isError: true);
                return;
            }

            if (!File.Exists(install.ClientAppSettingsFile))
            {
                SetStatus("No ClientAppSettings.json exists yet for this install (nothing to load).");
                return;
            }

            try
            {
                var entries = FlagFile.Load(install.ClientAppSettingsFile);
                _grid.Rows.Clear();
                foreach (var entry in entries)
                    _grid.Rows.Add(entry.Name, entry.ValueType, entry.RawValue);

                SetStatus($"Loaded {entries.Count} flag(s) from {install.ClientAppSettingsFile}");
            }
            catch (Exception ex)
            {
                SetStatus($"Failed to load: {ex.Message}", isError: true);
            }
        }

        private void ImportFromJson()
        {
            using var dialog = new OpenFileDialog
            {
                Title = "Import flags from JSON",
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            List<FlagEntry> imported;
            try
            {
                imported = FlagFile.Load(dialog.FileName);
            }
            catch (Exception ex)
            {
                SetStatus($"Failed to parse JSON: {ex.Message}", isError: true);
                return;
            }

            if (imported.Count == 0)
            {
                SetStatus("No flags found in that file (expected a flat JSON object of \"FlagName\": value pairs).", isError: true);
                return;
            }

            bool merge = true;
            if (_grid.Rows.Count > 0)
            {
                var choice = MessageBox.Show(
                    $"Found {imported.Count} flag(s) in the file.\n\n" +
                    "Yes = merge into the current list (update matching names, keep the rest)\n" +
                    "No = replace the current list entirely\n" +
                    "Cancel = abort import",
                    "Import From JSON",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (choice == DialogResult.Cancel)
                    return;

                merge = choice == DialogResult.Yes;
            }

            if (!merge)
            {
                _grid.Rows.Clear();
                foreach (var entry in imported)
                    _grid.Rows.Add(entry.Name, entry.ValueType, entry.RawValue);

                SetStatus($"Imported {imported.Count} flag(s) from {Path.GetFileName(dialog.FileName)} (replaced list).");
                return;
            }

            int updated = 0, added = 0;
            foreach (var entry in imported)
            {
                DataGridViewRow? existingRow = null;
                foreach (DataGridViewRow row in _grid.Rows)
                {
                    if (row.IsNewRow) continue;
                    var name = row.Cells["Name"].Value?.ToString() ?? "";
                    if (string.Equals(name, entry.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        existingRow = row;
                        break;
                    }
                }

                if (existingRow != null)
                {
                    existingRow.Cells["Type"].Value = entry.ValueType;
                    existingRow.Cells["Value"].Value = entry.RawValue;
                    updated++;
                }
                else
                {
                    _grid.Rows.Add(entry.Name, entry.ValueType, entry.RawValue);
                    added++;
                }
            }

            SetStatus($"Imported from {Path.GetFileName(dialog.FileName)}: {added} added, {updated} updated.");
        }

        private List<FlagEntry> CollectEntriesFromGrid()
        {
            var entries = new List<FlagEntry>();
            foreach (DataGridViewRow row in _grid.Rows)
            {
                if (row.IsNewRow) continue;

                string name = row.Cells["Name"].Value?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(name)) continue;

                var typeVal = row.Cells["Type"].Value;
                FlagValueType type = typeVal is FlagValueType fvt ? fvt : FlagValueType.String;
                string value = row.Cells["Value"].Value?.ToString() ?? "";

                entries.Add(new FlagEntry { Name = name.Trim(), ValueType = type, RawValue = value });
            }
            return entries;
        }

        private void SaveAndApply()
        {
            var entries = CollectEntriesFromGrid();
            if (entries.Count == 0)
            {
                var confirm = MessageBox.Show(
                    "The flag list is empty. This will write an empty settings file. Continue?",
                    "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm != DialogResult.Yes) return;
            }

            var targets = _applyToAllCheck.Checked
                ? _installs
                : (GetSelectedInstall() is RobloxInstall single ? new List<RobloxInstall> { single } : new List<RobloxInstall>());

            if (targets.Count == 0)
            {
                SetStatus("No target install selected.", isError: true);
                return;
            }

            int successCount = 0;
            var errors = new List<string>();

            foreach (var install in targets)
            {
                try
                {
                    RobloxLocator.EnsureClientSettingsFolder(install);
                    var backup = RobloxLocator.BackupFile(install.ClientAppSettingsFile);
                    FlagFile.Save(install.ClientAppSettingsFile, entries);
                    successCount++;
                }
                catch (Exception ex)
                {
                    errors.Add($"{install.VersionFolderName}: {ex.Message}");
                }
            }

            if (errors.Count == 0)
            {
                SetStatus($"Applied {entries.Count} flag(s) to {successCount} install(s). " +
                          "Restart Roblox for changes to take effect.");
            }
            else
            {
                SetStatus($"Applied to {successCount} install(s), but {errors.Count} failed: " +
                          string.Join("; ", errors), isError: true);
            }
        }

        private void SetStatus(string message, bool isError = false)
        {
            _statusLabel.Text = message;
            _statusLabel.ForeColor = isError ? System.Drawing.Color.Firebrick : System.Drawing.Color.DarkSlateGray;
        }
    }
}
